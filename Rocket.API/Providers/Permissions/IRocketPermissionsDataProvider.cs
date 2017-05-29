using System.Collections.ObjectModel;
using Rocket.API.Player;
using Rocket.API.Serialization;

namespace Rocket.API.Providers.Permissions
{
    [RocketProvider]
    public interface IRocketPermissionsDataProvider : IRocketDataProviderBase
    {
        ReadOnlyCollection<RocketPermissionsGroup> GetPlayerGroups(IRocketPlayer player);

        ReadOnlyCollection<RocketPermissionsGroup> GetPlayerGroups(string playerId);

        //ReadOnlyCollection<string> GetPermissions(IRocketPlayer player);

        //ReadOnlyCollection<string> GetPermissions(string playerId);

        RocketPermissionsGroup GetGroup(string groupId);

        bool AddGroup(RocketPermissionsGroup group);

        bool SaveGroup(RocketPermissionsGroup group);

        bool DeleteGroup(RocketPermissionsGroup group);

        PermissionResult CheckPermission(IRocketPlayer player, string permission);
        PermissionResult CheckPermission(string id, string permission);
    }
}