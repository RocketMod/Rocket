using System.Collections.Generic;
using Rocket.API.Commands;

namespace Rocket.API.Permissions
{
    public interface IPermissionProvider
    {
        bool HasPermission(IPermissionGroup group, string permission);
        bool HasPermission(ICommandCaller caller, string permission);

        bool HasAllPermissions(IPermissionGroup group, params string[] permissions);
        bool HasAllPermissions(ICommandCaller caller, params string[] permissions);

        bool HasAnyPermissions(IPermissionGroup group, params string[] permissions);
        bool HasAnyPermissions(ICommandCaller caller, params string[] permissions);

        IPermissionGroup GetPrimaryGroup(ICommandCaller caller);
        IEnumerable<IPermissionGroup> GetGroups(ICommandCaller caller);
    }
}