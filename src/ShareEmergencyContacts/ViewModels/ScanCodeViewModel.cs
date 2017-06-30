using ShareEmergencyContacts.Models.Data;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using ZXing;
using ZXing.Mobile;

namespace ShareEmergencyContacts.ViewModels
{
    public class ScanCodeViewModel : ViewModelBase
    {
        public ScanCodeViewModel()
        {
            ScanCommand = new Command(o => ScanResult(o?.ToString()));
            Options = new MobileBarcodeScanningOptions
            {
                PossibleFormats = new List<BarcodeFormat>
                {
                    BarcodeFormat.QR_CODE
                }
            };
        }

        public MobileBarcodeScanningOptions Options { get; }

        public ICommand ScanCommand { get; }

        public void ScanResult(string qrCode)
        {
            var p = ParseQrCode(qrCode);
            if (p != null)
            {

            }
        }

        private EmergencyProfile ParseQrCode(string qr)
        {
            return null;
        }
    }
}