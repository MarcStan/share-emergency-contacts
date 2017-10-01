using Xamarin.Forms;

namespace ShareEmergencyContacts.Views
{
    public partial class EditPhoneNumberView
    {
        public EditPhoneNumberView()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.UWP)
                NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}