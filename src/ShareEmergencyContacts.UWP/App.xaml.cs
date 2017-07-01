using Caliburn.Micro;
using ShareEmergencyContacts.Models;
using ShareEmergencyContacts.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;

namespace ShareEmergencyContacts.UWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
    {
        private readonly WinRTContainer _container;

        public App()
        {
            _container = new WinRTContainer();
            Initialize();
            InitializeComponent();
        }

        protected override void Configure()
        {
            _container.RegisterWinRTServices();

            _container.Singleton<ShareEmergencyContacts.App>();
            _container.RegisterInstance(typeof(IAppInfoProvider), null, new WindowsAppInfoProvider());
            _container.RegisterInstance(typeof(IStorageProvider), null, new WindowsStorageProvider());
            _container.RegisterInstance(typeof(IPhoneDialProvider), null, new WindowsPhoneDialProvider());
            _container.RegisterInstance(typeof(IClipboardProvider), null, new WindowsClipboardProvider());
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            _container.RegisterNavigationService(rootFrame);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Running)
                return;

            Xamarin.Forms.Forms.Init(args);

            DisplayRootView<MainPage>();
        }

        protected override IEnumerable<Assembly> SelectAssemblies()
        {
            return new[]
            {
                GetType().GetTypeInfo().Assembly,
                typeof(RootViewModel).GetTypeInfo().Assembly
            };
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}