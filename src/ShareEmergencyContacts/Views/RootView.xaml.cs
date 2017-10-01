using Xamarin.Forms;

namespace ShareEmergencyContacts.Views
{
    public partial class RootView
    {
        public RootView()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.UWP)
                NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
