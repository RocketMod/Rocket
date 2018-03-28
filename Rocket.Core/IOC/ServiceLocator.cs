using System;
using System.Collections.Generic;

namespace Rocket.Core.IOC
{
    internal class ServiceLocator : IServiceLocator
    {
        private Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator;

        public ServiceLocator(Microsoft.Practices.ServiceLocation.IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
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