using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.Handlers;

namespace Rocket.API.Permissions
{
    public interface IPermissionProvider : IHandler
    {
        // do not add something like GetPermissions()!

        bool CanHandle(ICommandCaller caller);

        EPermissionResult HasPermission(IPermissionGroup group, string permission);
        EPermissionResult HasPermission(ICommandCaller caller, string permission);

        EPermissionResult HasAllPermissions(IPermissionGroup group, params string[] permissions);
        EPermissionResult HasAllPermissions(ICommandCaller caller, params string[] permissions);

        EPermissionResult HasAnyPermissions(IPermissionGroup group, params string[] permissions);
        EPermissionResult HasAnyPermissions(ICommandCaller caller, params string[] permissions);

        bool AddPermission(IPermissionGroup group, string permission);

        bool AddInvertedPermission(IPermissionGroup group, string permission);

        bool AddPermission(ICommandCaller caller, string permission);

        bool AddInvertedPermission(ICommandCaller group, string permission);

        bool RemovePermission(IPermissionGroup group, string permission);

        bool RemoveInvertedPermission(IPermissionGroup group, string permission);

        bool RemovePermission(ICommandCaller caller, string permission);

        bool RemoveInvertedPermission(ICommandCaller group, string permission);

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