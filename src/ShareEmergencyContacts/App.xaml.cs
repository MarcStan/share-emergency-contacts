
using Caliburn.Micro;
using Caliburn.Micro.Xamarin.Forms;
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

            var ns = typeof(MainViewModel).Namespace;
            // auto register all view models
            RegisterAllViewModels(ns);

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
        /// Registers all classes that can be instantiated (non abstract, not interfaces) inside the namespace and all sub namespaces using per request type.
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

                _container.RegisterPerRequest(t, null, t);
            }
        }

        protected override void PrepareViewFirst(NavigationPage navigationPage)
        {
            _container.Instance<INavigationService>(new NavigationPageAdapter(navigationPage));
        }
    }
}
