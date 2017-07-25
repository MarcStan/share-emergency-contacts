using Acr.UserDialogs;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using ShareEmergencyContacts.Extensions;
using ShareEmergencyContacts.Models;
using ShareEmergencyContacts.ViewModels;
using ShareEmergencyContacts.Views;
using System;
using System.Diagnostics;
using System.Reflection;
using Xamarin.Forms;
using Device = Xamarin.Forms.Device;

namespace ShareEmergencyContacts
{
    public partial class App
    {
        private readonly SimpleContainer _container;

        public App(SimpleContainer container)
        {
            _container = container;

            InitializeComponent();
            Initialize();

            // XAML doesn't support conditionals so create a binding
#if BETA
            Resources.Add("IsInBeta", true);
#else
            Resources.Add("IsInBeta", false);
#endif
            var ns = typeof(RootViewModel).Namespace;
            // auto register all view models
            RegisterAllViewModels(ns);
            EnsurePlatformProvidersExist();

            var handler = IoC.Get<IUnhandledExceptionHandler>();
            handler.OnException += UnhandledException;

            MobileCenter();

            var storageProvider = IoC.Get<IStorageProvider>();
            container.RegisterInstance(typeof(IStorageContainer), null, new StorageContainer(storageProvider));

            DisplayRootView<RootView>();
        }

        private static void MobileCenter()
        {
            var platform = Device.RuntimePlatform.ToLower();
            var key = IoC.Get<IAppInfoProvider>().MobileCenterKey;

#if !DEBUG
            Microsoft.Azure.Mobile.MobileCenter.Start($"{platform}={key}", typeof(Analytics), typeof(Crashes));
#endif
        }

        private void UnhandledException(object sender, Exception exception)
        {
            if (Debugger.IsAttached)
                Debugger.Break();
            // TODO: do this: https://peterno.wordpress.com/2015/04/15/unhandled-exception-handling-in-ios-and-android-with-xamarin/
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
            EnsureExists<IUnhandledExceptionHandler>();
            EnsureExists<ICheckPermissions>();
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
                Debug.WriteLine(msg);
                throw new NotSupportedException(msg);
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
