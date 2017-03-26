using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API.Commands;
using Rocket.API.Player;

namespace Rocket.Core.Providers.Permissions
{
    public class RocketPermissionDataProviderProxy
    {
            //    public static bool HasPermission(this IRocketPlayer player, string permission)
            //    {
            //        return R.Permissions.HasPermission(player, permission);
            //    }

            //    public static bool HasPermissions(this IRocketPlayer player, IRocketCommand command)
            //    {
            //        return R.Permissions.HasPermission(player, command);
            //    }

            //    public static List<string> GetPermissions(this IRocketPlayer player)
            //    {
            //        return R.Permissions.GetPermissions(player);
            //    }
            //}
            //    public static bool HasPermission(this IRocketPermissionsDataProvider rocketPermissionProvider, IRocketPlayer player, string permission)
            //    {
            //        return rocketPermissionProvider.HasPermission(player, new List<string>() { permission });
            //    }

            //    public static bool HasPermission(this IRocketPermissionsDataProvider rocketPermissionProvider, IRocketPlayer player, IRocketCommand command)
            //    {
            //        List<string> requestedPermissions = command.Permissions;
            //        requestedPermissions.Add(command.Name);
            //        requestedPermissions.AddRange(command.Aliases);
            //        return rocketPermissionProvider.HasPermission(player, command);
            //    }

            //    public static bool HasPermission(this IRocketPermissionsDataProvider rocketPermissionProvider, IRocketPlayer player, List<string> requestedPermissions)
            //    {
            //        if (player.IsAdmin) return true;

            //        List<string> permissions = rocketPermissionProvider.GetPermissions(player);

            //        foreach (string permission in permissions.ToList())
            //        {
            //            if (permission.StartsWith("!"))
            //            {
            //                permissions.Remove(permission.Remove(0, 1));
            //                if (permission.EndsWith(".*"))
            //                {
            //                    permissions.RemoveAll(p => p.ToLower().StartsWith(permission.ToLower().TrimEnd('*')));
            //                }
            //                permissions.Remove(permission);
            //            }
            //        }



            //        foreach (string permission in requestedPermissions)
            //        {
            //            if (permission == "*") return true;

            //            if (permission.EndsWith(".*"))
            //            {
            //                if (requestedPermissions.Where(p => p.ToLower().StartsWith(permission.ToLower().TrimEnd('*'))).FirstOrDefault() != null) return true;
            //            }

            //            if (permissions.Where(p => p.ToLower() == permission.ToLower()) != null) return true;
            //        }

            //        return false;
            //    }

            //    public static bool AddPlayerToGroup(this IRocketPermissionsDataProvider rocketPermissionProvider, string group, IRocketPlayer player)
            //    {
            //        RocketPermissionsGroup g = rocketPermissionProvider.GetGroup(group);
            //        if (g != null)
            //        {
            //            if (!g.Members.Contains(player.Id))
            //            {
            //                g.Members.Add(player.Id);
            //                rocketPermissionProvider.SaveGroup(g);
            //            }
            //            return true;
            //        }
            //        else
            //        {
            //            return false;
            //        }
            //    }

            //    public static bool RemovePlayerFromGroup(this IRocketPermissionsDataProvider rocketPermissionProvider, string group, IRocketPlayer player)
            //    {
            //        RocketPermissionsGroup g = rocketPermissionProvider.GetGroup(group);
            //        if (g != null)
            //        {
            //            if (g.Members.Remove(player.Id))
            //            {
            //                rocketPermissionProvider.SaveGroup(g);
            //            }
            //            return true;
            //        }
            //        else
            //        {
            //            return false;
            //        }
            //    }
    }
}
