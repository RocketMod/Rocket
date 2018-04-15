using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.ServiceProxies;

namespace Rocket.Core.ServiceProxies
{
    public abstract class ServiceProxy<T> : IServiceProxy<T> where T: IProxyableService
    {
        private IDependencyContainer Container { get; }

        public IEnumerable<T> ProxiedProviders
        {
            get
            {
                var providers = Container.GetAll<T>()
                                         .Where(c => c.GetType() != GetType())
                                         .ToList();

                ServicePriorityComparer.Sort(providers, true);
                return providers;
            }
        }
        protected ServiceProxy(IDependencyContainer container)
        {
            if(!(this is T))
                throw new Exception("Service proxy has to extend " + typeof(T).FullName + "!");
            Container = container;
        }
    }
}