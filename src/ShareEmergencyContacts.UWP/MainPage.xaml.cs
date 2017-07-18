using Caliburn.Micro;

namespace ShareEmergencyContacts.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            LoadApplication(IoC.Get<ShareEmergencyContacts.App>());
        }
    }
}
