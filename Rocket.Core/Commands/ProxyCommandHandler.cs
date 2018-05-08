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

        public bool HandleCommand(IUser caller, string commandLine, string prefix)
        {
            GuardCaller(caller);

            foreach (ICommandHandler handler in ProxiedServices.Where(c => c.SupportsCaller(caller.GetType())))
                if (handler.HandleCommand(caller, commandLine, prefix))
                    return true;

            return false;
        }

        public bool SupportsCaller(Type User)
        {
            return ProxiedServices.Any(c => c.SupportsCaller(User));
        }

        private void GuardCaller(IUser caller)
        {
            if (!SupportsCaller(caller.GetType()))
                throw new NotSupportedException(caller.GetType().FullName + " is not supported!");
        }
    }
}