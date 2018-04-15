using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.ServiceProxies;

namespace Rocket.API.Permissions
{
    public interface IPermissionProvider : IProxyableService
    {
        // do not add something like GetPermissions()!

        bool SupportsCaller(ICommandCaller caller);
        bool SupportsGroup(IPermissionGroup group);

        PermissionResult HasPermission(IPermissionGroup group, string permission);
        PermissionResult HasPermission(ICommandCaller caller, string permission);

        PermissionResult HasAllPermissions(IPermissionGroup group, params string[] permissions);
        PermissionResult HasAllPermissions(ICommandCaller caller, params string[] permissions);

        PermissionResult HasAnyPermissions(IPermissionGroup group, params string[] permissions);
        PermissionResult HasAnyPermissions(ICommandCaller caller, params string[] permissions);

        bool AddPermission(IPermissionGroup group, string permission);

        bool AddDeniedPermission(IPermissionGroup group, string permission);

        bool AddPermission(ICommandCaller caller, string permission);

        bool AddDeniedPermission(ICommandCaller caller, string permission);

        bool RemovePermission(IPermissionGroup group, string permission);

        bool RemoveDeniedPermission(IPermissionGroup group, string permission);

        bool RemovePermission(ICommandCaller caller, string permission);

        bool RemoveDeniedPermission(ICommandCaller caller, string permission);

        IPermissionGroup GetPrimaryGroup(ICommandCaller caller);

        IPermissionGroup GetGroup(string id);

        IEnumerable<IPermissionGroup> GetGroups(ICommandCaller caller);
        IEnumerable<IPermissionGroup> GetGroups();

        void UpdateGroup(IPermissionGroup group);

        void AddGroup(ICommandCaller caller, IPermissionGroup group);

        bool RemoveGroup(ICommandCaller caller, IPermissionGroup group);

        void CreateGroup(IPermissionGroup @group);

        void DeleteGroup(IPermissionGroup @group);

        void Load(IConfigurationElement groups, IConfigurationElement players);
        void Reload();
        void Save();
    }
}