using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.ViewModels;
using System;
using Xamarin.Forms;

namespace ShareEmergencyContacts.Views
{
    public partial class EditProfileView
    {
        public EditProfileView()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.UWP)
                NavigationPage.SetHasNavigationBar(this, false);
            // disable back button because we display cancel
            NavigationPage.SetHasBackButton(this, false);
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = BindingContext as EditProfileViewModel;
            if (vm != null)
            {
                var h = new BackButtonHelper();
                h.OnResult += (s, cancel) =>
                {
                    if (cancel)
                    {
                        IoC.Get<INavigationService>().GoBackAsync();
                    }
                };
                vm.CancelBackButton(h);
                // always return true since this can't be async and thus won't wait for execution of CancelBackButton
                // true means cancel the back button request
                base.OnBackButtonPressed();
                return true;
            }
            return base.OnBackButtonPressed();
        }

        public class BackButtonHelper
        {
            /// <summary>
            /// Invoked once a result is set.
            /// </summary>
            public event EventHandler<bool> OnResult;

            public void SetResult(bool p0)
            {
                OnResult?.Invoke(this, p0);
            }
        }

        private void MenuItem_OnClicked(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }

        private void TextEntryCompleted(object sender, EventArgs e)
        {
            // TODO: only Editor fields (multiline) will call this method; Entry fields do not fire the Completed event (bug in xamarin?)
            var vm = BindingContext as EditProfileViewModel;
            vm?.TextEntryCompleted();
        }
    }
}