using Acr.UserDialogs;
using Caliburn.Micro;
using ShareEmergencyContacts.Models;
using ShareEmergencyContacts.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;
using ZXing.Net.Mobile.Forms.WindowsUniversal;

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
            _container.RegisterInstance(typeof(IAppInfoProvider), null, new WindowsAppInfoProvider("a5db724f-2ea9-4421-ade8-612e5dccb2aa"));
            _container.RegisterInstance(typeof(IStorageProvider), null, new WindowsStorageProvider());
            _container.RegisterInstance(typeof(IPhoneDialProvider), null, new WindowsPhoneDialProvider());
            _container.RegisterInstance(typeof(IClipboardProvider), null, new WindowsClipboardProvider());
            _container.RegisterInstance(typeof(IShareProvider), null, new WindowsShareProvider());
            _container.RegisterInstance(typeof(IUserDialogs), null, UserDialogs.Instance);
            _container.RegisterInstance(typeof(IUnhandledExceptionHandler), null, new WindowsUnhandledExceptionHandler());
            _container.RegisterInstance(typeof(ICheckPermissions), null, new WindowsCheckPermissions());
        }

        protected override void PrepareViewFirst(Frame rootFrame)
        {
            _container.RegisterNavigationService(rootFrame);
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Running ||
                args.PreviousExecutionState == ApplicationExecutionState.Suspended)
                return;

            ZXingScannerViewRenderer.Init();
            var refs = new List<Assembly>
            {
                typeof(ZXingBarcodeImageViewRenderer).GetTypeInfo().Assembly
            };
            Xamarin.Forms.Forms.Init(args, refs);

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