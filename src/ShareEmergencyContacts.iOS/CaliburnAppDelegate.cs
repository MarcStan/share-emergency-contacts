﻿using Caliburn.Micro;
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
            _container.RegisterInstance(typeof(IAppInfoProvider), null, new IOSAppInfoProvider());
            _container.RegisterInstance(typeof(IStorageProvider), null, new IOSStorageProvider());
            _container.RegisterInstance(typeof(IPhoneDialProvider), null, new IOSPhoneDialProvider());
            _container.RegisterInstance(typeof(IClipboardProvider), null, new IOSClipboardProvider());
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