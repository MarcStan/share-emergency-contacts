using Acr.UserDialogs;
using Caliburn.Micro;
using ShareEmergencyContacts.Models.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using ZXing;
using ZXing.Mobile;

namespace ShareEmergencyContacts.ViewModels
{
    /// <summary>
    /// View model for the screen to scan barcodes.
    /// As soon as it is created it will scan barcodes and continuously callback for each found barcode.
    /// </summary>
    public class ScanCodeViewModel : Screen
    {
        private readonly Func<EmergencyProfile, Task<bool>> _add;
        private bool _finished;
        private bool _isAnalyzing;
        private bool _isScanning;

        public ScanCodeViewModel(Func<EmergencyProfile, Task<bool>> add)
        {
            _add = add ?? throw new ArgumentNullException(nameof(add));

            ScanCommand = new Command(o =>
            {
                if (o != null)
                    ScanResult(o.ToString());
            });
            Options = new MobileBarcodeScanningOptions
            {
                PossibleFormats = new List<BarcodeFormat>
                {
                    BarcodeFormat.QR_CODE
                }
            };
            IsScanning = IsAnalyzing = true;
        }

        public MobileBarcodeScanningOptions Options { get; }

        public ICommand ScanCommand { get; }

        public bool IsScanning
        {
            get => _isScanning;
            set
            {
                if (value == _isScanning) return;
                _isScanning = value;
                NotifyOfPropertyChange(nameof(IsScanning));
            }
        }

        public bool IsAnalyzing
        {
            get => _isAnalyzing;
            set
            {
                if (value == _isAnalyzing) return;
                _isAnalyzing = value;
                NotifyOfPropertyChange(nameof(IsAnalyzing));
            }
        }

        /// <summary>
        /// Called for each barcode found.
        /// </summary>
        /// <param name="qrCode"></param>
        public void ScanResult(string qrCode)
        {
            // stop analyzing further barcodes
            IsAnalyzing = false;

            if (_finished)
                return;

            Device.BeginInvokeOnMainThread(async () =>
            {
                var p = EmergencyProfile.ParseFromText(qrCode);
                if (p != null)
                {
                    if (_finished)
                        return;
                    _finished = true;
                    // on match, exit but not before registering the new file
                    if (!await _add(p))
                    {
                        // user canceled and discarded the file
                        // keep scanning
                        _finished = false;
                        IsAnalyzing = true;
                    }
                }
                else
                {
                    IoC.Get<IUserDialogs>().Alert("Not a valid contact format. Please try again!", "Invalid format");
                    // continue anlyzing
                    IsAnalyzing = true;
                }
            });
        }
    }
}