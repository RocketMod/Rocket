using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.Permissions;
using Rocket.API.ServiceProxies;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Commands
{
    public class ProxyCommandHandler : ServiceProxy<ICommandHandler>, ICommandHandler
    {
        public ProxyCommandHandler(IDependencyContainer container) : base(container) { }

        public bool HandleCommand(ICommandCaller caller, string commandLine)
        {
            GuardCaller(caller);

            foreach (var handler in ProxiedProviders.Where(c => c.SupportsCaller(caller)))
            {
                if (handler.HandleCommand(caller, commandLine))
                {
                    return true;
                }
            }

            return false;
        }

        private void GuardCaller(ICommandCaller caller)
        {
            if(!SupportsCaller(caller))
                throw new NotSupportedException(caller.GetType().FullName + " is not supported!");
        }

        public bool SupportsCaller(ICommandCaller caller)
        {
            return ProxiedProviders.Any(c => c.SupportsCaller(caller));
        }

        public ICommand GetCommand(ICommandContext ctx)
        {
            GuardCaller(ctx.Caller);

            foreach (var provider in ProxiedProviders)
            {
                var prov = provider.GetCommand(ctx);
                if (prov != null)
                    return prov;
            }

            return null;
        }
    }
}