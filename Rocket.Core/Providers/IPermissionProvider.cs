using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rocket.Core.Tuples;

namespace Rocket.Core.Providers
{
    //Going to need a PermissionGroup object.
    public interface IPermissionProvider
    {
        Permission[] GetPlayerPermissions(IPlayer player);
        Tuple<IPlayer, Permission[]>[] GetPlayersPermissions(IPlayer[] players);
    }
}
