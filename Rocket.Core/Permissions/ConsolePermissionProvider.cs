using Rocket.API.Commands;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Permissions
{
    [ServicePriority(Priority = ServicePriority.Highest)]
    public class ConsolePermissionProvider : FullPermitPermissionProvider<IConsole>
    {

    }
}
