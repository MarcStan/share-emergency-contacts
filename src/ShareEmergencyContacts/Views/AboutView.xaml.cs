using Xamarin.Forms;

namespace ShareEmergencyContacts.Views
{
    public partial class AboutView
    {
        public AboutView()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.UWP)
                NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
