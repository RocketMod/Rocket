using Rocket.API.Configuration;
using Rocket.API.DependencyInjection;
using Rocket.API.Permissions;
using Rocket.Core.ServiceProxies;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Rocket.Core.Permissions
{
    public class PermissionProviderProxy : ServiceProxy<IPermissionChecker>, IPermissionChecker
    {
        public PermissionProviderProxy(IDependencyContainer container) : base(container) { }

        public bool SupportsTarget(IPermissionEntity target)
        {
            return ProxiedServices.Any(c => c.SupportsTarget(target));
        }

        public async Task<PermissionResult> CheckPermissionAsync(IPermissionEntity target, string permission)
        {
            GuardTarget(target);

            foreach (IPermissionChecker provider in ProxiedServices.Where(c => c.SupportsTarget(target)))
            {
                PermissionResult result = await provider.CheckPermissionAsync(target, permission);
                if (result == PermissionResult.Default)
                    continue;

                return result;
            }

            return PermissionResult.Default;
        }

        private void GuardTarget(IPermissionEntity target)
        {
            if (!SupportsTarget(target))
                throw new NotSupportedException(target.GetType().FullName + " is not supported!");
        }

        public string ServiceName => "ProxyPermissions";
    }
}