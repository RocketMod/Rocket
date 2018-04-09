using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Configuration;

namespace Rocket.API.Permissions
{
    public interface IPermissionProvider
    {
        // do not add something like GetPermissions()!

        bool HasPermission(IPermissionGroup group, string permission);
        bool HasPermission(ICommandCaller caller, string permission);

        bool HasAllPermissions(IPermissionGroup group, params string[] permissions);
        bool HasAllPermissions(ICommandCaller caller, params string[] permissions);

        bool HasAnyPermissions(IPermissionGroup group, params string[] permissions);
        bool HasAnyPermissions(ICommandCaller caller, params string[] permissions);

        IPermissionGroup GetPrimaryGroup(ICommandCaller caller);

        IPermissionGroup GetGroup(string id);

        IEnumerable<IPermissionGroup> GetGroups(ICommandCaller caller);
        IEnumerable<IPermissionGroup> GetGroups();

        void UpdateGroup(IPermissionGroup group);

        void AddGroup(ICommandCaller caller, IPermissionGroup group);

        bool RemoveGroup(ICommandCaller caller, IPermissionGroup group);

        void CreateGroup(IPermissionGroup @group);

        void DeleteGroup(IPermissionGroup @group);

        void Load(IConfiguration groups, IConfiguration players);
        void Reload();
        void Save();
    }
}