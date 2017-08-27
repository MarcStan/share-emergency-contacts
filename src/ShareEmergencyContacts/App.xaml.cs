﻿using Acr.UserDialogs;
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;
using PCLStorage;
using ShareEmergencyContacts.Extensions;
using ShareEmergencyContacts.Helpers;
using ShareEmergencyContacts.Models;
using ShareEmergencyContacts.ViewModels;
using ShareEmergencyContacts.Views;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using Device = Xamarin.Forms.Device;

namespace ShareEmergencyContacts
{
    public partial class App
    {
        private readonly SimpleContainer _container;

        public App(SimpleContainer container)
        {
            EnsurePlatformProvidersExist();

            var handler = IoC.Get<IUnhandledExceptionHandler>();
            handler.OnException += UnhandledException;
#if DEBUG
            LoadAndDisplayException();
#endif
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

            Task.Run(async () =>
            {
#if !DEBUG
                // don't report stuff from my debug sessions or there will be lots of crash reports to ignore
                MobileCenter();
#endif
                // call after mobile center start because start seems to set all variables to true
                await AppSettings.LoadAsync();
            });
            var storageProvider = IoC.Get<IStorageProvider>();
            container.RegisterInstance(typeof(IStorageContainer), null, new StorageContainer(storageProvider));

            DisplayRootView<RootView>();
        }

        private static void MobileCenter()
        {
            var platform = Device.RuntimePlatform.ToLower();
            if (Device.RuntimePlatform == Device.Windows)
                platform = "uwp"; // Xamarin used to call it windows, will call it UWP as of 2.3-pre6 but I'm still on 2.3-stable

            var key = IoC.Get<IAppInfoProvider>().MobileCenterKey;
            // mobile center doesn't support UWP crashes yet
            if (platform == "uwp")
                Microsoft.Azure.Mobile.MobileCenter.Start($"{platform}={key}", typeof(Analytics));
            else
                Microsoft.Azure.Mobile.MobileCenter.Start($"{platform}={key}", typeof(Analytics), typeof(Crashes));
        }

        private void UnhandledException(object sender, Exception exception)
        {
            if (!Debugger.IsAttached)
            {
                var t = Task.Run(async () =>
                {
                    var root = FileSystem.Current.LocalStorage;
                    var file = await root.CreateFileAsync("lastrun.exception.txt", CreationCollisionOption.ReplaceExisting);
                    var formatted = exception.ToString();
                    await file.WriteAllTextAsync(formatted);
                });
                Task.WaitAll(t);
            }
            else
            {
                // by breaking we sometimes get more detail for the exception; esp. on android
                Debugger.Break();
            }
        }

        private void LoadAndDisplayException()
        {
            Task.Run(async () =>
            {
                var root = FileSystem.Current.LocalStorage;
                if (await root.CheckExistsAsync("lastrun.exception.txt") == ExistenceCheckResult.FileExists)
                {
                    var file = await root.GetFileAsync("lastrun.exception.txt");
                    var content = await file.ReadAllTextAsync();
                    await file.DeleteAsync();
                    await IoC.Get<IUserDialogs>().AlertAsync(content, "Last uncaught crash", "Ok");
                }
            });
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
