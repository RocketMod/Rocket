using Rocket.API.Commands;
using Rocket.API.Serialisation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Rocket.API.Player;

namespace Rocket.API.Providers.Permissions
{
    [RocketProvider]
    public interface IRocketPermissionsDataProvider : IRocketDataProviderBase
    {
        ReadOnlyCollection<RocketPermissionsGroup> GetGroups(IRocketPlayer player);

        ReadOnlyCollection<RocketPermissionsGroup> GetGroups(string id);

        //ReadOnlyCollection<string> GetPermissions(IRocketPlayer player);

        //ReadOnlyCollection<string> GetPermissions(string id);

        RocketPermissionsGroup GetGroup(string groupId);

        bool AddGroup(RocketPermissionsGroup group);

        bool SaveGroup(RocketPermissionsGroup group);

        bool DeleteGroup(RocketPermissionsGroup group);

        bool HasPermission(IRocketPlayer player, string permission);
        bool HasPermission(string id, string permission);
    }
}