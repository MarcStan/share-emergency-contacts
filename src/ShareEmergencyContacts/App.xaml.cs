using Acr.UserDialogs;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Extensions;
using ShareEmergencyContacts.Models;
using ShareEmergencyContacts.ViewModels;
using ShareEmergencyContacts.Views;
using System;
using System.Reflection;
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

            var ns = typeof(RootViewModel).Namespace;
            // auto register all view models
            RegisterAllViewModels(ns);

            EnsurePlatformProvidersExist();

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

        /// <summary>
        /// Call once to throw on any platform that failed to provide an implementation of a required provider.
        /// </summary>
        private static void EnsurePlatformProvidersExist()
        {
            EnsureExists<IAppInfoProvider>();
            EnsureExists<IStorageProvider>();
            EnsureExists<IPhoneDialProvider>();
            EnsureExists<IClipboardProvider>();
            EnsureExists<IShareProvider>();
            EnsureExists<IUserDialogs>();
        }

        private static void EnsureExists<T>()
        {
            bool throw_ = false;
            try
            {
                if (IoC.Get<T>() == null)
                    throw_ = true;

            }
            catch (Exception)
            {
                throw_ = true;
            }
            if (throw_)
            {
                // exception is swallowed (maybe because in ctor?) and throws an unrelated (with no info) InvocationException 
                // so write to debug as well
                var msg = $"No implementation of {typeof(T)} has been provided on {Device.RuntimePlatform}.";
                System.Diagnostics.Debug.WriteLine(msg);
                throw new NotImplementedException(msg);
            }
        }

        /// <summary>
        /// Registers all classes that can be instantiated (non abstract, not interfaces) inside the namespace and all sub namespaces using per request type.
        /// Only registeres classes with name of format: *ViewModel
        /// </summary>
        /// <param name="nameSpace"></param>
        private void RegisterAllViewModels(string nameSpace)
        {
            var types = GetType().GetTypeInfo().Assembly.ExportedTypes;
            foreach (var t in types)
            {
                var ti = t.GetTypeInfo();
                if (!ti.Namespace.StartsWith(nameSpace))
                    continue;

                if (ti.IsAbstract || ti.IsInterface)
                    continue;

                if (!ti.Name.EndsWith("ViewModel"))
                    continue;

                _container.RegisterPerRequest(t, null, t);
            }
        }

        protected override void PrepareViewFirst(NavigationPage navigationPage)
        {
            _container.Instance<INavigationService>(new ExtendedNavPageAdapter(navigationPage));
        }
    }
}
