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

            foreach (ICommandHandler handler in ProxiedServices.Where(c => c.SupportsCaller(caller.GetType())))
                if (handler.HandleCommand(caller, commandLine, prefix))
                    return true;

            return false;
        }

        public bool SupportsCaller(Type commandCaller)
        {
            return ProxiedServices.Any(c => c.SupportsCaller(commandCaller));
        }

        private void GuardCaller(ICommandCaller caller)
        {
            if (!SupportsCaller(caller.GetType()))
                throw new NotSupportedException(caller.GetType().FullName + " is not supported!");
        }
    }
}