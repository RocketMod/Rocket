using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Commands
{
    public class ProxyCommandProvider : ServiceProxy<ICommandProvider>, ICommandProvider
    {
        public ProxyCommandProvider(IDependencyContainer container) : base(container) { }

        public ILifecycleObject GetOwner(ICommand command)
        {
            foreach(var service in ProxiedServices)
                if (service.Commands.Any(c => c == command))
                    return service.GetOwner(command);

            throw new Exception("Owner not found");
        }

        public IEnumerable<ICommand> Commands =>
            ProxiedServices.SelectMany(c => c.Commands);
    }
}