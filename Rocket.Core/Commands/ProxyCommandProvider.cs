using System.Collections.Generic;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Commands
{
    public class ProxyCommandProvider : ServiceProxy<ICommandProvider>, ICommandProvider
    {
        public ProxyCommandProvider(IDependencyContainer container) : base(container) { }
        public IEnumerable<ICommand> Commands => 
            ProxiedProviders.SelectMany(c => c.Commands);
    }
}