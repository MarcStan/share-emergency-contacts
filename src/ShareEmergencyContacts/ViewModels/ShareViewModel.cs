using ZXing.QrCode;

namespace ShareEmergencyContacts.ViewModels
{
    public class ShareViewModel : ViewModelBase
    {
        public QrCodeEncodingOptions Options { get; }

        public string Barcode { get; }
    }
}