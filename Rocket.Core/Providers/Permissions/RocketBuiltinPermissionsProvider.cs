using Rocket.API.Player;
using Rocket.API.Providers.Permissions;
using Rocket.API.Serialisation;
using System;
using Rocket.API.Assets;
using System.Collections.ObjectModel;
using System.Linq;
using Rocket.API.Providers.Logging;

namespace Rocket.Core.Providers.Permissions
{
    public sealed class RocketBuiltinPermissionsProvider : IRocketPermissionsDataProvider
    {
        private RocketPermissionsHelper helper;
        
        public ReadOnlyCollection<RocketPermissionsGroup> GetGroups(IRocketPlayer player)
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

        public bool HasPermission(IRocketPlayer player, string permission)
        {
            //todo ??
            return
                GetPermissions(player)
                    .Any(c => c.Equals(permission, StringComparison.OrdinalIgnoreCase) && !c.StartsWith("!"));
        }

        public ReadOnlyCollection<RocketPermissionsGroup> GetGroups(string id)
        {
            return helper.GetGroups(id).AsReadOnly();
        }

        public ReadOnlyCollection<string> GetPermissions(string id)
        {
            return helper.GetPermissions(id).AsReadOnly();
        }

        public bool HasPermission(string id, string permission)
        {
            return
                GetPermissions(id)
                    .Any(c => c.Equals(permission, StringComparison.OrdinalIgnoreCase) && !c.StartsWith("!"));
        }
    }
}
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