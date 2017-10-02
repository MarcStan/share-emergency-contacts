using Android.App;
using Android.Runtime;
using Caliburn.Micro;
using ShareEmergencyContacts.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace ShareEmergencyContacts.Droid
{
#if DEBUG
    [Application(Debuggable = true)]
#else
    [Application(Debuggable = false)]
#endif
    public class CaliburnApplication : Caliburn.Micro.CaliburnApplication
    {
        /// <summary>
        /// Use a static instance because this application class is only ever instantiated once: on the first app launch.
        /// If a user closes the app (pushed to background) and clicks the app icon again, a new app is launched.
        /// BUT: this application is not created again, only the MainActivity.OnCreate/OnResume functions are called
        /// </summary>
        public static SimpleContainer Container;

        public CaliburnApplication(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            Initialize();
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
            Container.BuildUp(instance);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return Container.GetAllInstances(service);
        }

        protected override object GetInstance(Type service, string key)
        {
            return Container.GetInstance(service, key);
        }
    }
}