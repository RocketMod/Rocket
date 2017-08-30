using System.Collections.ObjectModel;
using Rocket.API.Player;
using Rocket.API.Serialization;
using Rocket.API.Permissions;

namespace Rocket.API.Providers
{
    [ProviderDefinition]
    public interface IPermissionsProvider 
    {
        ReadOnlyCollection<RocketPermissionsGroup> GetPlayerGroups(IPlayer player);

        ReadOnlyCollection<RocketPermissionsGroup> GetPlayerGroups(string playerId);

        //ReadOnlyCollection<string> GetPermissions(IRocketPlayer player);

        //ReadOnlyCollection<string> GetPermissions(string playerId);

        RocketPermissionsGroup GetGroup(string groupId);

        bool AddGroup(RocketPermissionsGroup group);

        bool SaveGroup(RocketPermissionsGroup group);

        bool DeleteGroup(RocketPermissionsGroup group);

        PermissionResult CheckPermission(IPlayer player, string permission);
        PermissionResult CheckPermission(string id, string permission);
        void Save();
    }
}