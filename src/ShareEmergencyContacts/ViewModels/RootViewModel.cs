namespace ShareEmergencyContacts.ViewModels
{
    public class RootViewModel : ViewModelBase
    {
        private bool _menuIsPresented;

        public RootViewModel()
        {
            MenuViewModel = new MenuViewModel(this);
        }

        public MenuViewModel MenuViewModel { get; set; }

        public bool MenuIsPresented
        {
            get { return _menuIsPresented; }
            set
            {
                _menuIsPresented = value;
                NotifyOfPropertyChange(nameof(MenuIsPresented));
            }
        }
    }
}
