using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;

namespace ShareEmergencyContacts.ViewModels
{
    public class RootViewModel : ViewModelBase
    {
        private bool _menuIsPresented;

        public RootViewModel()
        {
            var navigationService = IoC.Get<INavigationService>();
            MenuViewModel = new MenuViewModel(navigationService, this);
            MainViewModel = new MainViewModel(navigationService);
        }

        public MenuViewModel MenuViewModel { get; set; }

        public MainViewModel MainViewModel { get; set; }

        public bool MenuIsPresented
        {
            get { return _menuIsPresented; }
            set
            {
                _menuIsPresented = value;
                NotifyOfPropertyChange(nameof(MenuIsPresented));
            }
        }

        protected override async void OnActivate()
        {
            base.OnActivate();
            // because mainpage is page inside rootpage, mainpage never receives activated/etc events
            // so use this workaround
            await MainViewModel.OnPageActivateAsync();
        }
    }
}
