using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using System.Windows.Input;
using Xamarin.Forms;
#if BETA
using Acr.UserDialogs;
using Newtonsoft.Json;
using System;
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
            _navigationService = IoC.Get<INavigationService>();
            MyProfilesViewModel = new MyProfilesViewModel(_navigationService);
            ReceivedContactsViewModel = new ReceivedContactsViewModel(_navigationService);

            AboutCommand = new Command(About);
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

            var response = await client.GetAsync($"api/sec/update/?v={v}&p={p}");
            if (!response.IsSuccessStatusCode)
                return; // silent fail

            var json = await response.Content.ReadAsStringAsync();
            try
            {
                var update = JsonConvert.DeserializeObject<UpdateCheck>(json);
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
            catch
            {
                // silent ignore again
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

        public void About()
        {
            _navigationService.NavigateToViewModelAsync<AboutViewModel>();
        }
    }
}
