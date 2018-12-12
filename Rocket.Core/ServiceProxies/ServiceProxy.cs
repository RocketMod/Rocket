using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.DependencyInjection;

namespace Rocket.Core.ServiceProxies
{
    public abstract class ServiceProxy<T> : IServiceProxy<T> where T : class, IProxyableService
    {
        protected ServiceProxy(IDependencyContainer container)
        {
            if (!(this is T))
                throw new Exception($"{GetType().FullName} service proxy has to inherit from {typeof(T).FullName}!");

            Container = container;
        }

        private IDependencyContainer Container { get; }

        public IEnumerable<T> ProxiedServices
        {
            get
            {
                List<T> providers = Container
                                    .ResolveAll<T>()
                                    .Where(c => !c.GetType().GetCustomAttributes(typeof(DontProxyAttribute), false).Any())
                                    .Distinct()
                                    .ToList();

                ServicePriorityComparer.Sort(providers, true);
                return providers;
            }
        }
    }
}