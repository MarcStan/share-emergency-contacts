using Android.App;
using Android.Runtime;
using Caliburn.Micro;
using ShareEmergencyContacts.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ShareEmergencyContacts.Droid
{
    [Application]
    public class Application : CaliburnApplication
    {
        private readonly SimpleContainer _container;

        public Application(IntPtr javaReference, JniHandleOwnership transfer)
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
            _container.RegisterInstance(typeof(IAppInfoProvider), null, new AndroidAppInfoProvider());
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