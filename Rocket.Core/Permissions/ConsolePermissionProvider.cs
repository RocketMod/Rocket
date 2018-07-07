using System;
using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.Permissions;
using Rocket.API.User;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Permissions
{
    [ServicePriority(Priority = ServicePriority.Highest)]
    public class ConsolePermissionProvider : FullPermitPermissionProvider<IConsole>
    {

    }
}
