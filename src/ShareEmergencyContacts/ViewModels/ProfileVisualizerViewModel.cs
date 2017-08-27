using Caliburn.Micro;
using Microsoft.Azure.Mobile.Analytics;
using ShareEmergencyContacts.Helpers;
using ShareEmergencyContacts.Models.Data;
using ShareEmergencyContacts.ViewModels.ForModels;
using System;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using ZXing.QrCode;

namespace ShareEmergencyContacts.ViewModels
{
    /// <summary>
    /// Viewmodel for a single selected profile with all its emergency contacts and 
    /// </summary>
    public class ProfileVisualizerViewModel : Screen
    {
        private ProfileViewModel _selected;
        private string _barcodeContent;
        private bool _first = true;
        private BindableCollection<string> _shareDurations;
        private string _selectedShareDuration;
        private readonly int[] _duration;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="deleteAction"></param>
        /// <param name="editAction"></param>
        /// <param name="showBarcodeFirst">If true, shows the barcode fullscreen first.</param>
        public ProfileVisualizerViewModel(ProfileViewModel profile, Action<ProfileViewModel> deleteAction, Action<ProfileViewModel> editAction, bool showBarcodeFirst = false, bool allowShareDuration = false)
        {
            Selected = profile ?? throw new ArgumentNullException(nameof(profile));
            ShowBarcodeFirst = showBarcodeFirst;
            CanEdit = editAction != null;
            CanDelete = deleteAction != null;

            var appInfo = IoC.Get<IAppInfoProvider>();
            // margin of 10 around each side of the screen so make it roughly the size of the screen
            // however limit to reasonable pixel size because it's a fucking QR code. it can't have aliasing artifacts
            // because all lines are axis aligned
            // no point in having a 2048*2048 barcode on those ridiculous modern screens
            var s = Math.Min(1024, appInfo.ScreenWidth - 20);
            Options = new QrCodeEncodingOptions
            {
                Width = s,
                Height = s
            };
            EditCommand = new Command(() =>
            {
                if (!CanEdit)
                    return;
                editAction(Selected);
            });
            DeleteCommand = new Command(() =>
            {
                if (!CanDelete)
                    return;
                deleteAction(Selected);
            });
            ShareDurations = new BindableCollection<string>(new[] { "1 day", "3 days", "7 days", "2 weeks", "1 month", "3 month", "6 month", "unlimited" });
            _duration = new[] { 1, 3, 7, 14, 28, 28 * 3, 28 * 6, -1 };
            SelectedShareDuration = ShareDurations.Last();
            CanChangeShareDuration = allowShareDuration;
        }

        public bool CanChangeShareDuration { get; }

        public string ActiveShareDuration
        {
            get
            {
                if (CanChangeShareDuration)
                    return null;

                var exp = Selected.Actual.ExpirationDate;
                if (exp.HasValue)
                {
                    var day = exp.Value;
                    var n = DateTime.Now;
                    var delta = day - n;
                    if (delta.TotalDays < 7)
                    {
                        var d = (int)delta.TotalDays + 1;

                        return $"Contact is shared for {d} more day{(d == 1 ? "" : "s")}";
                    }
                    return $"Contact is shared until {day:M}";
                }

                return "Contact is shared forever";
            }
        }

        public ICommand EditCommand { get; }

        public ICommand DeleteCommand { get; }

        public string BarcodeContent
        {
            get => _barcodeContent;
            private set
            {
                if (value == _barcodeContent) return;
                _barcodeContent = value;
                NotifyOfPropertyChange(nameof(BarcodeContent));
            }
        }

        public bool ShowBarcodeFirst { get; }

        public bool CanEdit { get; }

        public bool CanDelete { get; }

        public QrCodeEncodingOptions Options { get; }

        public ProfileViewModel Selected
        {
            get => _selected;
            private set
            {
                if (Equals(value, _selected)) return;
                _selected = value;
                NotifyOfPropertyChange(nameof(Selected));
            }
        }

        public BindableCollection<string> ShareDurations
        {
            get => _shareDurations;
            set
            {
                if (Equals(value, _shareDurations)) return;
                _shareDurations = value;
                NotifyOfPropertyChange(nameof(ShareDurations));
            }
        }

        public string SelectedShareDuration
        {
            get => _selectedShareDuration;
            set
            {
                if (value == _selectedShareDuration) return;
                _selectedShareDuration = value;
                int duration = _duration[ShareDurations.IndexOf(SelectedShareDuration)];

                var clone = Selected.Actual.CloneFull();
                // set on cloned value, and don't modify the real data
                // because we don't want the value saved in our own profiles (the value is meant for the share receiver only)
                clone.ExpirationDate = duration == -1 ? (DateTime?)null : DateTime.Now.AddDays(duration);

                UpdateBarcode(clone);
                NotifyOfPropertyChange(nameof(SelectedShareDuration));
            }
        }

        protected override void OnActivate()
        {
            UpdateBarcode(Selected.Actual);
            if (!_first)
            {
                Selected.UpdateLists();
                Selected.Refresh();
                foreach (var e in Selected.EmergencyContacts)
                {
                    e.Refresh();
                    foreach (var p in e.PhoneNumbers)
                    {
                        p.Refresh();
                    }
                }
                foreach (var i in Selected.InsuranceContacts)
                {
                    i.Refresh();
                    foreach (var p in i.PhoneNumbers)
                    {
                        p.Refresh();
                    }
                }
                foreach (var p in Selected.PhoneNumbers)
                {
                    p.Refresh();
                }
                Refresh();
            }
            _first = false;
            base.OnActivate();
        }

        private void UpdateBarcode(EmergencyProfile p)
        {
            BarcodeContent = EmergencyProfile.ToRawText(p);
        }

        public void PageChanged(bool share)
        {
            // showbarcodefirst is true for profile view as its more likely that user wants to share his profiles vs looking contacts
            if (share)
            {
                if (ShowBarcodeFirst)
                    Analytics.TrackEvent(AnalyticsEvents.ShareProfile);
                else
                    Analytics.TrackEvent(AnalyticsEvents.ShareContact);
            }
            else
            {
                if (ShowBarcodeFirst)
                    Analytics.TrackEvent(AnalyticsEvents.ShowEditProfile);
                else
                    Analytics.TrackEvent(AnalyticsEvents.ShowEditContact);
            }
        }
    }
}