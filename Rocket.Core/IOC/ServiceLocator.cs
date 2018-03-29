using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Rocket.API.IOC;

namespace Rocket.Core.IOC
{
    internal class UnityServiceLocator : IServiceLocator
    {
        private readonly Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator;

        public UnityServiceLocator(IUnityContainer container)
        {
            this.serviceLocator = new Microsoft.Practices.Unity.UnityServiceLocator(container);
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return serviceLocator.GetAllInstances(serviceType);
        }

        public IEnumerable<TService> GetAllInstances<TService>()
        {
            return serviceLocator.GetAllInstances<TService>();
        }

        public object GetInstance(Type serviceType)
        {
            return serviceLocator.GetInstance(serviceType);
        }

        public object GetInstance(Type serviceType, string key)
        {
            return serviceLocator.GetInstance(serviceType, key);
        }

        public TService GetInstance<TService>()
        {
            return serviceLocator.GetInstance<TService>();
        }

        public TService GetInstance<TService>(string key)
        {
            return serviceLocator.GetInstance<TService>(key);
        }

        public object GetService(Type serviceType)
        {
            return serviceLocator.GetService(serviceType);
        }
    }
}