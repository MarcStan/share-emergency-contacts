using Caliburn.Micro;

namespace ShareEmergencyContacts.UWP
{
    public sealed partial class MainPage
    {
        public static MainPage Instance { get; private set; }

        public MainPage()
        {
            Instance = this;
            InitializeComponent();

            LoadApplication(IoC.Get<ShareEmergencyContacts.App>());
        }
    }
}
