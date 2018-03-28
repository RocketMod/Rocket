using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Rocket.Core
{
    //Todo: Going to need a PermissionGroup object.
    public interface IPermissionProvider
    {
        bool HasPermissions(IPlayer player);
    }
}
