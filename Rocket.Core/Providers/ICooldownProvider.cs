using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Rocket.Core.Tuples;

namespace Rocket.Core.Providers
{
    //This would be useful to allow different methods that Rocket could handle and store a cooldown. i.e. MySQL, XML, JSON, LiteDB
    public interface ICooldownProvider
    {
        Tuple<IPlayer, Cooldown[]> GetAwaitingCooldowns();
        Tuple<IPlayer, Cooldown[]>[] GetAwaitingCooldowns(IPlayer[] player);

        Cooldown[] GetAwaitingCooldowns(IPlayer player);

        bool? CheckPermissionReady(IPlayer player, Permission permission);

        Tuple<IPlayer, bool?[]>[] CheckPermissionsReady(IPlayer[] players, Permission[] permissions);

        void RegisterCooldown(IPlayer player, Permission permission);
        void RegisterCooldowns(IPlayer[] players, Permission[] permission);

        void RemoveCooldowns(IPlayer[] players, Permission[] permissions);

        void RemoveAllCooldowns(Permission[] permission);
        void RemoveAllCooldowns(IPlayer[] player);
    }
}
