using Android.App;
using Android.Runtime;
using Caliburn.Micro;
using ShareEmergencyContacts.Models;
using ShareEmergencyContacts.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ShareEmergencyContacts.Droid
{
    [Application]
    public class CaliburnApplication : Caliburn.Micro.CaliburnApplication
    {
        private readonly SimpleContainer _container;

        public CaliburnApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            _container = new SimpleContainer();
        }

        public override void OnCreate()
        {
            base.OnCreate();

            Initialize();
        }

        protected override void Configure()
        {
            _container.Instance(_container);
            _container.Singleton<App>();
            _container.RegisterInstance(typeof(IStorageProvider), null, new AndroidIOSStorageProvider());
            _container.RegisterInstance(typeof(IPhoneDialProvider), null, new AndroidPhoneDialProvider());
            _container.RegisterInstance(typeof(IClipboardProvider), null, new AndroidClipboardProvider());
            _container.RegisterInstance(typeof(IShareProvider), null, new AndroidShareProvider());
            _container.RegisterInstance(typeof(IUnhandledExceptionHandler), null, new AndroidUnhandledExceptionHandler());
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[]
            {
                GetType().Assembly,
                typeof (RootViewModel).Assembly
            };
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
    }
}