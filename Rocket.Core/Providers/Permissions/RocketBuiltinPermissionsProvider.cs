using Rocket.API.Player;
using Rocket.API.Providers.Permissions;
using System;
using Rocket.API.Assets;
using System.Collections.ObjectModel;
using Rocket.API.Providers;
using Rocket.API.Providers.Logging;
using Rocket.API.Serialization;

namespace Rocket.Core.Providers.Permissions
{
    [RocketProviderImplementation]
    public sealed class RocketBuiltinPermissionsProvider : IRocketPermissionsDataProvider
    {
        private RocketPermissionsHelper helper;
        
        public ReadOnlyCollection<RocketPermissionsGroup> GetPlayerGroups(IRocketPlayer player)
        {
            return helper.GetGroups(player).AsReadOnly();
        }

        public ReadOnlyCollection<string> GetPermissions(IRocketPlayer player)
        {
            return helper.GetPermissions(player).AsReadOnly();
        }

        public RocketPermissionsGroup GetGroup(string groupId)
        {
            return helper.GetGroup(groupId);
        }

        public bool AddGroup(RocketPermissionsGroup group)
        {
            return helper.AddGroup(group);
        }

        public bool SaveGroup(RocketPermissionsGroup group)
        {
            return helper.SaveGroup(group);
        }

        public bool DeleteGroup(RocketPermissionsGroup group)
        {
            return helper.DeleteGroup(group);
        }
        
        public void Save()
        {
            helper.permissions.Save();
        }

        public void Unload(bool isReload = false)
        {
            helper.permissions.Unload();
        }

        public void Load(bool reloading)
        {
            try
            {
                helper = new RocketPermissionsHelper(new XMLFileAsset<RocketPermissions>(PermissionFile));
            }
            catch (Exception ex)
            {
                R.Logger.Log(LogLevel.FATAL, ex);
            }
        }

        public string PermissionFile => "Permissions.config.xml";

        public PermissionResult CheckPermission(IRocketPlayer player, string permission)
        {
            PermissionResult result = new PermissionResult(PermissionResultType.DEFAULT, PermissionPriority.LOW);

            foreach (var perm in GetPermissions(player))
            {
                if (perm.Equals(permission, StringComparison.OrdinalIgnoreCase) && result.Result != PermissionResultType.DENY)
                {
                    result.Result = PermissionResultType.GRANT;
                }

                if (perm.Substring(1).Equals(permission, StringComparison.OrdinalIgnoreCase) && perm.StartsWith("!"))
                {
                    result.Result = PermissionResultType.DENY;
                }
            }

            return result;
        }

        public ReadOnlyCollection<RocketPermissionsGroup> GetPlayerGroups(string playerId)
        {
            return helper.GetGroups(playerId).AsReadOnly();
        }

        public ReadOnlyCollection<string> GetPermissions(string id)
        {
            return helper.GetPermissions(id).AsReadOnly();
        }

        public PermissionResult CheckPermission(string id, string permission)
        {
            PermissionResult result = new PermissionResult(PermissionResultType.DEFAULT, PermissionPriority.LOW);
            
            foreach (var perm in GetPermissions(id))
            {
                if (perm.Equals(permission, StringComparison.OrdinalIgnoreCase) && result.Result != PermissionResultType.DENY)
                {
                    result.Result = PermissionResultType.GRANT;        
                }

                if (perm.Substring(1).Equals(permission, StringComparison.OrdinalIgnoreCase) && perm.StartsWith("!"))
                {
                    result.Result = PermissionResultType.DENY;
                }
            }

            return result;
        }
    }
}
//    public static bool CheckPermission(this IRocketPlayer player, string permission)
//    {
//        return R.Permissions.CheckPermission(player, permission);
//    }

//    public static bool HasPermissions(this IRocketPlayer player, IRocketCommand command)
//    {
//        return R.Permissions.CheckPermission(player, command);
//    }

//    public static List<string> GetPermissions(this IRocketPlayer player)
//    {
//        return R.Permissions.GetPermissions(player);
//    }
//}
//    public static bool CheckPermission(this IRocketPermissionsDataProvider rocketPermissionProvider, IRocketPlayer player, string permission)
//    {
//        return rocketPermissionProvider.CheckPermission(player, new List<string>() { permission });
//    }

//    public static bool CheckPermission(this IRocketPermissionsDataProvider rocketPermissionProvider, IRocketPlayer player, IRocketCommand command)
//    {
//        List<string> requestedPermissions = command.Permissions;
//        requestedPermissions.Add(command.Name);
//        requestedPermissions.AddRange(command.Aliases);
//        return rocketPermissionProvider.CheckPermission(player, command);
//    }

//    public static bool CheckPermission(this IRocketPermissionsDataProvider rocketPermissionProvider, IRocketPlayer player, List<string> requestedPermissions)
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