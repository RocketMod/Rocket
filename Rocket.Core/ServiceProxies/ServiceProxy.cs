using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Rocket.API.DependencyInjection;
using Rocket.API.User;
using Rocket.Core.User;

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