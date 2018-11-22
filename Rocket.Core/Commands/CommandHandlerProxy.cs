using System;
using System.Linq;
using System.Threading.Tasks;
using Rocket.API.Commands;
using Rocket.API.DependencyInjection;
using Rocket.API.User;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Commands
{
    public class CommandHandlerProxy : ServiceProxy<ICommandHandler>, ICommandHandler
    {
        public CommandHandlerProxy(IDependencyContainer container) : base(container) { }

        public async Task<bool> HandleCommandAsync(IUser user, string commandLine, string prefix)
        {
            GuardUser(user);

            foreach (ICommandHandler handler in ProxiedServices.Where(c => c.SupportsUser(user)))
                if (await handler.HandleCommandAsync(user, commandLine, prefix))
                    return true;

            return false;
        }

        public bool SupportsUser(IUser user)
        {
            return ProxiedServices.Any(c => c.SupportsUser(user));
        }

        public string GetPermission(ICommandContext context)
        {
            foreach (var handler in ProxiedServices)
            {
                var perm = handler.GetPermission(context);
                if (perm != null)
                    return perm;
            }

            return null;
        }

        private void GuardUser(IUser user)
        {
            if (!SupportsUser(user))
                throw new NotSupportedException(user.GetType().FullName + " is not supported!");
        }

        public string ServiceName => "ProxyCommandHandler";
    }
}