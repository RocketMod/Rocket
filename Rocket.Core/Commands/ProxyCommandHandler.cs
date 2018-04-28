using System;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Commands
{
    public class ProxyCommandHandler : ServiceProxy<ICommandHandler>, ICommandHandler
    {
        public ProxyCommandHandler(IDependencyContainer container) : base(container) { }

        public bool HandleCommand(ICommandCaller caller, string commandLine, string prefix)
        {
            GuardCaller(caller);

            foreach (var handler in ProxiedServices.Where(c => c.SupportsCaller(caller.GetType())))
            {
                if (handler.HandleCommand(caller, commandLine, prefix))
                {
                    return true;
                }
            }

            return false;
        }

        private void GuardCaller(ICommandCaller caller)
        {
            if(!SupportsCaller(caller.GetType()))
                throw new NotSupportedException(caller.GetType().FullName + " is not supported!");
        }

        public bool SupportsCaller(Type commandCaller)
        {
            return ProxiedServices.Any(c => c.SupportsCaller(commandCaller));
        }

        public ICommand GetCommand(ICommandContext context)
        {
            GuardCaller(context.Caller);

            foreach (var provider in ProxiedServices)
            {
                var prov = provider.GetCommand(context);
                if (prov != null)
                    return prov;
            }

            return null;
        }
    }
}