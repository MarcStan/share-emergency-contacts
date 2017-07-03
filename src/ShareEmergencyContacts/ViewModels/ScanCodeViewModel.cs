using Acr.UserDialogs;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Models.Data;
using System;
using System.Collections.Generic;
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
    public class ScanCodeViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly Action<EmergencyProfile> _add;
        private bool _finished;
        private bool _isAnalyzing;
        private bool _isScanning;

        public ScanCodeViewModel(INavigationService navigationService, Action<EmergencyProfile> add)
        {
            _add = add ?? throw new ArgumentNullException(nameof(add));

            _navigationService = navigationService;
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

            Device.BeginInvokeOnMainThread(() =>
            {
                var p = EmergencyProfile.ParseFromText(qrCode);
                if (p != null)
                {
                    if (_finished)
                        return;
                    _finished = true;
                    IsScanning = false;
                    // on match, exit but not before registering the new file
                    _add(p);
                    _navigationService.GoBackAsync();
                }
                else
                {
                    using (IoC.Get<IUserDialogs>().Alert("Not a valid contact! Please scan the barcode from another user!", "Invalid format"))
                    {

                    }
                    // continue anlyzing
                    IsAnalyzing = true;
                }
            });
        }
    }
}