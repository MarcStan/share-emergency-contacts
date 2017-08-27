using System.Linq;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using Microsoft.Azure.Mobile.Analytics;
using ShareEmergencyContacts.Helpers;
using System.Windows.Input;
using ShareEmergencyContacts.Models;
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
            ExportCommand = new Command(Export);
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

        public void About()
        {
            Analytics.TrackEvent(AnalyticsEvents.OpenAbout);
            _navigationService.NavigateToViewModelAsync<AboutViewModel>();
        }

        public async void Export()
        {
            var c = ReceivedContactsViewModel.ExistingContacts.Count;
            var p = MyProfilesViewModel.ExistingContacts.Count;
            var dia = IoC.Get<IUserDialogs>();
            if (!await dia.ConfirmAsync($"Really export {c} contact{(c == 1 ? "" : "s")} and {p} profile{(p == 1 ? "" : "s")}?", "Export all?", "yes", "no"))
                return;

            var storage = IoC.Get<IStorageProvider>();
            var csv = ImportExportHelper.ToFile(ReceivedContactsViewModel.ExistingContacts.Select(s => s.Actual).ToList(), MyProfilesViewModel.ExistingContacts.Select(s => s.Actual).ToList());

            var n = DateTime.Now;
            var date = $"{n.Year}-{n.Month:00}-{n.Day:00}";
            await storage.SaveExternallyAsync($"{date}-contacts-export.vcards", csv);
            if (Device.RuntimePlatform == Device.Android)
                dia.Toast("Contacts exported to Downloads folder!");
            else
                dia.Toast("Contacts exported!");
        }
    }
}
