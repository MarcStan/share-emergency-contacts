﻿using Caliburn.Micro;
using Microsoft.Azure.Mobile.Analytics;
using ShareEmergencyContacts.Helpers;
using ShareEmergencyContacts.Models.Data;
using ShareEmergencyContacts.ViewModels.ForModels;
using System;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="deleteAction"></param>
        /// <param name="editAction"></param>
        /// <param name="showBarcodeFirst">If true, shows the barcode fullscreen first.</param>
        public ProfileVisualizerViewModel(ProfileViewModel profile, Action<ProfileViewModel> deleteAction, Action<ProfileViewModel> editAction, bool showBarcodeFirst = false)
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

        protected override void OnActivate()
        {
            BarcodeContent = EmergencyProfile.ToRawText(Selected.Actual);
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