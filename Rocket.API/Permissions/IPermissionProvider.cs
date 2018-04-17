using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.ServiceProxies;

namespace Rocket.API.Permissions
{
    public interface IPermissionProvider : IProxyableService
    {
        // do not add something like GetPermissions()!

        bool SupportsPermissible(IPermissible permissible);

        PermissionResult CheckPermission(IPermissible target, string permission);

        PermissionResult CheckHasAllPermissions(IPermissible target, params string[] permissions);

        PermissionResult CheckHasAnyPermission(IPermissible target, params string[] permissions);

        bool AddPermission(IPermissible target, string permission);

        bool AddDeniedPermission(IPermissible target, string permission);

        bool RemovePermission(IPermissible target, string permission);

        bool RemoveDeniedPermission(IPermissible target, string permission);

        IPermissionGroup GetPrimaryGroup(ICommandCaller caller);

        IPermissionGroup GetGroup(string id);

        IEnumerable<IPermissionGroup> GetGroups(IPermissible target);
        IEnumerable<IPermissionGroup> GetGroups();

        void UpdateGroup(IPermissionGroup group);

        bool AddGroup(IPermissible caller, IPermissionGroup group);

        bool RemoveGroup(IPermissible caller, IPermissionGroup group);

        bool CreateGroup(IPermissionGroup @group);

        bool DeleteGroup(IPermissionGroup @group);

        void Load(IConfigurationElement groups, IConfigurationElement players);
        void Reload();
        void Save();
    }
}