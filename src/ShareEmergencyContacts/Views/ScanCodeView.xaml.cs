using Xamarin.Forms;

namespace ShareEmergencyContacts.Views
{
    public partial class ScanCodeView
    {
        public ScanCodeView()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.UWP)
                NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}