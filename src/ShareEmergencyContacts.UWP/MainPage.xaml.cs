using Caliburn.Micro;

namespace ShareEmergencyContacts.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            ZXing.Net.Mobile.Forms.WindowsUniversal.ZXingScannerViewRenderer.Init();
            LoadApplication(IoC.Get<ShareEmergencyContacts.App>());
        }
    }
}
