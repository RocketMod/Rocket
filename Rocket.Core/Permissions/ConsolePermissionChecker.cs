using System.Threading.Tasks;
using Rocket.API.Commands;
using Rocket.API.Permissions;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Permissions
{
    [ServicePriority(Priority = ServicePriority.Highest)]
    public class ConsolePermissionChecker : IPermissionChecker
    {
        public string ServiceName { get; } = "ConsolePermissions";
        public bool SupportsTarget(IPermissionActor target) => target is IConsole;

        public Task<PermissionResult> CheckPermissionAsync(IPermissionActor target, string permission) => Task.FromResult(PermissionResult.Grant);
    }
}
