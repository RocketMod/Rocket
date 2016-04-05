using Rocket.API.Serialisation;
using Rocket.Core.Assets;
using System.Collections.Generic;

namespace Rocket.API
{
    public interface IRocketPermissionsProvider
    {
        bool HasPermission(IRocketPlayer player, string permission,  bool defaultReturnValue = false);
        bool HasPermission(IRocketPlayer player, IRocketCommand command, out uint? cooldownLeft, bool defaultReturnValue = false);
        bool HasPermission(IRocketPlayer player, List<string> requestedPermissions, out uint? cooldownLeft, bool defaultReturnValue = false);

        List<RocketPermissionsGroup> GetGroups(IRocketPlayer player, bool includeParentGroups);

        List<Permission> GetPermissions(IRocketPlayer player);

        bool SetGroup(IRocketPlayer player, string groupID);

        void Reload();
    }
}