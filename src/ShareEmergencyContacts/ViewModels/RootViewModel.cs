using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using Microsoft.Azure.Mobile.Analytics;
using ShareEmergencyContacts.Helpers;
using System.Windows.Input;
using ShareEmergencyContacts.Models;
using ShareEmergencyContacts.Models.Data;
using Xamarin.Forms;
#if BETA
using Acr.UserDialogs;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
#endif

namespace ShareEmergencyContacts.ViewModels
{
    public class RootViewModel : Screen
    {
        private readonly INavigationService _navigationService;

        public RootViewModel()
        {
            Analytics.TrackEvent(AnalyticsEvents.AppLaunch);

            _navigationService = IoC.Get<INavigationService>();
            MyProfilesViewModel = new MyProfilesViewModel(_navigationService);
            ReceivedContactsViewModel = new ReceivedContactsViewModel(_navigationService);

            AboutCommand = new Command(About);
            ExportCommand = new Command(ExportToFile);
            ImportCommand = new Command(ImportFromFile);
#if BETA
            PerformBetaUpdateCheck();
#endif
        }

#if BETA
        /// <summary>
        /// Checks in with the server to determine whether the current version is outdated.
        /// Will silently fail on any error and just prompt the user on success if a new version is available.
        /// </summary>
        private async void PerformBetaUpdateCheck()
        {
            var p = Device.RuntimePlatform.ToLower();
            var v = GetType().GetTypeInfo().Assembly.GetName().Version.ToString(3);
            var client = new HttpClient
            {
                BaseAddress = new Uri("https://marcstan.net/")
            };
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            try
            {
                var response = await client.GetAsync($"api/sec/update/{p}/{v}");
                if (!response.IsSuccessStatusCode)
                    return; // silent fail

                var json = await response.Content.ReadAsStringAsync();
                var update = JsonParser.FromJson<UpdateCheck>(json);
                if (update.UpdateAvailable)
                {
                    var dia = IoC.Get<IUserDialogs>();
                    var r = await dia.ConfirmAsync("A new update is available. Do you want to download it now?", "Update available", "Yes", "No");
                    if (r)
                    {
                        Device.OpenUri(new Uri(update.UpdateUrl, UriKind.Absolute));
                    }
                }
            }
            catch (Exception e)
            {
                // silently ignore all
                Debug.WriteLine(e);
                if (Debugger.IsAttached)
                    Debugger.Break();
            }
        }

        private class UpdateCheck
        {
            /// <summary>
            /// Gets whether a new update is available relative to the currently installed one.
            /// </summary>
            public bool UpdateAvailable { get; set; }

            /// <summary>
            /// The link where the new update can be loaded from
            /// </summary>
            public string UpdateUrl { get; set; }
        }
#endif

        public MyProfilesViewModel MyProfilesViewModel { get; set; }

        public ReceivedContactsViewModel ReceivedContactsViewModel { get; set; }

        public ICommand AboutCommand { get; }

        public ICommand ExportCommand { get; }

        public ICommand ImportCommand { get; }

        public void About()
        {
            Analytics.TrackEvent(AnalyticsEvents.OpenAbout);
            _navigationService.NavigateToViewModelAsync<AboutViewModel>();
        }

        public async void ImportFromFile()
        {
            if (!await EnsureStorageAccess())
                return;

            var storage = IoC.Get<IStorageProvider>();
            var dia = IoC.Get<IUserDialogs>();
            string content;
            try
            {
                content = await storage.ReadExternallyAsync(".vcards");
            }
            catch (Exception e)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    dia.Alert(e.Message);
                });
                return;
            }
            if (content == null)
                return;
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (!ImportExportHelper.FromFile(lines, out IList<EmergencyProfile> contacts,
                out IList<EmergencyProfile> profiles))
            {
                dia.Toast("Import failed!");
                return;
            }
            // don't overwrite existing names, just create a new file with a unique number
            int imp = 0;
            var storageContainer = IoC.Get<IStorageContainer>();
            foreach (var c in contacts)
            {
                var n = ReceivedContactsViewModel.Create(c);
                int id = 1;
                var uniqueName = n.ProfileName;
                while (ReceivedContactsViewModel.ExistingContacts.Any(p => p.ProfileName == uniqueName))
                {
                    id++;
                    uniqueName = n.ProfileName + $" {id}";
                }
                n.ProfileName = uniqueName;
                ReceivedContactsViewModel.ExistingContacts.Add(n);
                await storageContainer.SaveReceivedContactAsync(n.Actual);
                imp++;
            }
            foreach (var p in profiles)
            {
                var n = MyProfilesViewModel.Create(p);
                int id = 1;
                var uniqueName = n.ProfileName;
                while (MyProfilesViewModel.ExistingContacts.Any(p2 => p2.ProfileName == uniqueName))
                {
                    id++;
                    uniqueName = n.ProfileName + $" {id}";
                }
                n.ProfileName = uniqueName;
                MyProfilesViewModel.ExistingContacts.Add(n);
                await storageContainer.SaveProfileAsync(n.Actual);
                imp++;
            }
            dia.Toast($"{imp} contact{(imp == 1 ? "" : "s")} imported!");
        }

        public async void ExportToFile()
        {
            var c = ReceivedContactsViewModel.ExistingContacts.Count;
            var p = MyProfilesViewModel.ExistingContacts.Count;
            var dia = IoC.Get<IUserDialogs>();
            if (!await dia.ConfirmAsync($"Really export {c} contact{(c == 1 ? "" : "s")} and {p} profile{(p == 1 ? "" : "s")}?", "Export all?", "yes", "no"))
                return;

            if (!await EnsureStorageAccess())
                return;

            var storage = IoC.Get<IStorageProvider>();
            var file = ImportExportHelper.ToFile(ReceivedContactsViewModel.ExistingContacts.Select(s => s.Actual).ToList(), MyProfilesViewModel.ExistingContacts.Select(s => s.Actual).ToList());

            var n = DateTime.Now;
            var date = $"{n.Year}-{n.Month:00}-{n.Day:00}";
            await storage.SaveExternallyAsync($"{date}-contacts-export.vcards", file);
            if (Device.RuntimePlatform == Device.Android)
                dia.Toast("Contacts exported to Downloads folder!");
            else
                dia.Toast("Contacts exported!");
        }

        private async Task<bool> EnsureStorageAccess()
        {
            var permCheck = IoC.Get<ICheckPermissions>();
            var grantResult = await permCheck.GrantPermissionAsync(PermissionType.Storage);
            switch (grantResult)
            {
                case PermissionResult.Granted:
                    return true;
                case PermissionResult.Denied:
                    return false;
                case PermissionResult.AlwaysDenied:
                    // user won't even be prompted anymore with a dialog
                    // since this is the main feature of the app this will confuse any user who accidently set "never ask again"
                    // therefore tell him how to fix it
                    var dia = IoC.Get<IUserDialogs>();
                    string navPath;
                    var appName = "Share emergency contacts";
                    switch (Device.RuntimePlatform)
                    {
                        case Device.Android:
                            navPath = $"Apps -> {appName} -> Permissions";
                            break;
                        case Device.Windows:
                            // doesn't even exist on UWP, so we should never get access denied
                            navPath = "Storage";
                            break;
                        case Device.iOS:
                            navPath = "Storage";
                            break;
                        default:
                            throw new NotSupportedException($"Unsupported platform '{Device.RuntimePlatform}'.");
                    }
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        dia.Alert($"You have permanently denied access to the camera previously. To use this feature again, please go to 'Settings -> {navPath}' and manually enable camera access.", "Camera access denied");
                    });
                    return false;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
