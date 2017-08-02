using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.ViewModels;
using System;

namespace ShareEmergencyContacts.Views
{
    public partial class EditProfileView
    {
        public EditProfileView()
        {
            InitializeComponent();
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
    }
}