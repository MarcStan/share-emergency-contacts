using Caliburn.Micro.Xamarin.Forms;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly RootViewModel _root;

        public MenuViewModel(INavigationService navigationService, RootViewModel root)
        {
            _navigationService = navigationService;
            _root = root;
            AboutCommand = new Command(ShowAbout);
        }

        public ICommand AboutCommand { get; }

        public void ShowMyProfiles()
        {


        }

        public void ShowSettings()
        {

        }

        public void ShowAbout()
        {
            CloseMenuAndsNavigateTo<AboutViewModel>();
        }

        /// <summary>
        /// Navigates to the specific page after closing the menu
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private async void CloseMenuAndsNavigateTo<T>()
        {

            if (Device.RuntimePlatform == Device.Windows &&
                _root.MenuIsPresented)
            {
                // workaround for menu bug on uwp
                // on iOS and android, back navigation works as expected and the flyout menu is still open

                // pressing back on uwp for the first time seems to closes the menu but since it is already invisible (due to being overlayed by another page) the user feels like "nothing happened"
                // pressing it a second time will then return to the original page with the flyout closed
                // as a workaround, we close it ourself
                _root.MenuIsPresented = false;
                // but if that was all it would be too easy!
                // if we navigate directly afterwards xamarin crashes with "Height cannot be negative number (prob. because the fade out animation uses page height or some crap)
                // so just delay for a bit
                // but if we don't delay long enough there is another bug, hah!
                // this time, the back navigation will work fine but when the user returns to the master page (the menu flyout will be gone) the entire page will be unresponsive
                // because xamarin still thinks the flyout is over the page (prob. some internal invisible grid to prevent click-throughs when in the menu)
                // sooo we wait for a very long time (the time it takes the menu to close + some extra time)
                await Task.Delay(200);
            }

            await _navigationService.NavigateToViewModelAsync<T>();
        }
    }
}
