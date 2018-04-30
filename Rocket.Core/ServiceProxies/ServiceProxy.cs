using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.DependencyInjection;

namespace Rocket.Core.ServiceProxies
{
    public abstract class ServiceProxy<T> : IServiceProxy<T> where T : IProxyableService
    {
        protected ServiceProxy(IDependencyContainer container)
        {
            if (!(this is T))
                throw new Exception("Service proxy has to extend " + typeof(T).FullName + "!");
            Container = container;
        }

        private IDependencyContainer Container { get; }

        public IEnumerable<T> ProxiedServices
        {
            get
            {
                List<T> providers = Container.GetAll<T>()
                                             .ToList();

                ServicePriorityComparer.Sort(providers, true);
                return providers;
            }
        }
    }
}