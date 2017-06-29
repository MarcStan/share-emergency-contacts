using System.Collections.Generic;
using ZXing;

namespace ShareEmergencyContacts.Views
{
    public partial class ScanCodeView
    {
        public ScanCodeView()
        {
            InitializeComponent();
            SetupScan();
        }

        private void SetupScan()
        {
            var options = new ZXing.Mobile.MobileBarcodeScanningOptions();
            options.PossibleFormats = new List<BarcodeFormat>
            {
                BarcodeFormat.QR_CODE
            };

            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            scanner.TopText = "Scan the qr code from the other phone";
            scanner.ScanContinuously(options, r =>
            {
                if (r.Text != null)
                {
                    var qr = r.Text;
                    scanner.Cancel();
                }
            });


        }
    }
}