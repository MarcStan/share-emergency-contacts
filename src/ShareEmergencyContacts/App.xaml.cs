
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Models;
using ShareEmergencyContacts.ViewModels;
using ShareEmergencyContacts.Views;
using System;
using Xamarin.Forms;

namespace ShareEmergencyContacts
{
    public partial class App
    {
        private readonly SimpleContainer _container;

        public App(SimpleContainer container)
        {
            Initialize();

            _container = container;
            InitializeComponent();

            container.PerRequest<RootViewModel>()
                .PerRequest<AboutViewModel>()
                .PerRequest<MainViewModel>()
                .PerRequest<MenuViewModel>()
                .PerRequest<MyProfilesViewModel>()
                .PerRequest<SettingsViewModel>();

            var storageProvider = IoC.Get<IStorageProvider>();
            container.RegisterInstance(typeof(IStorageContainer), null, new StorageContainer(storageProvider));

            var original = ViewLocator.LocateForModelType;
            ViewLocator.LocateForModelType += (o, bindableObject, arg3) =>
            {
                var view = original(o, bindableObject, arg3);
                var label = view as Label;
                if (label != null)
                {
                    // on all platforms Caliburn.Micro will display an error message when view resolve failed
                    // but on xamarin forms this will crash because they don't return a proper page but only a label instead
                    throw new NotSupportedException("Caliburn.Micro failed to resolve the view, here's the details: " + label.Text);
                }
                return view;
            };

            DisplayRootView<RootView>();
        }

        protected override void PrepareViewFirst(NavigationPage navigationPage)
        {
            _container.Instance<INavigationService>(new NavigationPageAdapter(navigationPage));
        }
    }
}
