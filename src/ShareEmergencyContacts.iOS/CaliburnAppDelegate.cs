using Acr.UserDialogs;
using Caliburn.Micro;
using Foundation;
using ShareEmergencyContacts.Droid;
using ShareEmergencyContacts.Models;
using ShareEmergencyContacts.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ShareEmergencyContacts.iOS
{
    public class CaliburnAppDelegate : CaliburnApplicationDelegate
    {
        private readonly SimpleContainer _container;

        public CaliburnAppDelegate()
        {
            _container = new SimpleContainer();
            Initialize();
        }

        protected override void Configure()
        {
            _container.Instance(_container);
            _container.Singleton<App>();

            var v = NSBundle.MainBundle.InfoDictionary.ValueForKey(new NSString("CFBundleURLSchemes"));
            var key = v.ToString().Substring("mobilecenter-".Length);
            _container.RegisterInstance(typeof(IAppInfoProvider), null, new IOSAppInfoProvider(key));
            _container.RegisterInstance(typeof(IStorageProvider), null, new AndroidIOSStorageProvider());
            _container.RegisterInstance(typeof(IPhoneDialProvider), null, new IOSPhoneDialProvider());
            _container.RegisterInstance(typeof(IClipboardProvider), null, new IOSClipboardProvider());
            _container.RegisterInstance(typeof(IShareProvider), null, new IOSShareProvider());
            _container.RegisterInstance(typeof(IUserDialogs), null, UserDialogs.Instance);
            _container.RegisterInstance(typeof(IUnhandledExceptionHandler), null, new IOSUnhandledExceptionHandler());
            _container.RegisterInstance(typeof(ICheckPermissions), null, new IOSCheckPermissions());
            _container.RegisterInstance(typeof(IThemeProvider), null, new IOSThemeProvider());
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[]
            {
                GetType().Assembly,
                typeof(RootViewModel).Assembly
            };
        }
    }
}