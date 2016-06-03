using Rocket.API.Serialisation;
using System.Collections.Generic;

namespace Rocket.API
{
    public enum RocketPermissionsProviderResult { Success, UnspecifiedError, DuplicateEntry, GroupNotFound,PlayerNotFound };

    public interface IRocketPermissionsProvider
    {
        bool HasPermission(IRocketPlayer player, string permission,  bool defaultReturnValue = false);
        bool HasPermission(IRocketPlayer player, IRocketCommand command, out uint? cooldownLeft, bool defaultReturnValue = false);
        bool HasPermission(IRocketPlayer player, List<string> requestedPermissions, out uint? cooldownLeft, bool defaultReturnValue = false);

        List<RocketPermissionsGroup> GetGroups(IRocketPlayer player, bool includeParentGroups);
        List<Permission> GetPermissions(IRocketPlayer player);
        RocketPermissionsProviderResult AddPlayerToGroup(string groupId, IRocketPlayer player);
        RocketPermissionsProviderResult RemovePlayerFromGroup(string groupId, IRocketPlayer player);

        RocketPermissionsGroup GetGroup(string groupId);
        RocketPermissionsProviderResult AddGroup(RocketPermissionsGroup group);
        RocketPermissionsProviderResult SaveGroup(RocketPermissionsGroup group);
        RocketPermissionsProviderResult DeleteGroup(string groupId);

        void Reload();
    }
}