using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.Handlers;
using Rocket.API.Permissions;

namespace Rocket.Core.Permissions
{
    [HandlerPriority(Priority = HandlerPriority.Lowest)]
    public class PermissionProvider : IPermissionProvider
    {
        public IConfigurationElement GroupsConfig { get; protected set; }
        public IConfigurationElement PlayersConfig { get; protected set; }

        public bool CanHandle(ICommandCaller caller)
        {
            return true;
        }

        public EPermissionResult HasPermission(IPermissionGroup @group, string permission)
        {
            GuardLoaded();
            GuardPermission(ref permission);

            if (!permission.StartsWith("!") && HasPermission(@group, "!" + permission) == EPermissionResult.Grant)
                return EPermissionResult.Deny;

            var permissionTree = BuildPermissionTree(permission);
            foreach (var permissionNode in permissionTree)
            {
                string[] groupPermissions = GetConfigSection(@group)["Permissions"].Get(new string[0]);
                if (groupPermissions.Any(c => c.Trim().Equals(permissionNode, StringComparison.OrdinalIgnoreCase)))
                    return EPermissionResult.Grant;
            }

            return EPermissionResult.Default;
        }

        public EPermissionResult HasPermission(ICommandCaller caller, string permission)
        {
            if (caller is IConsoleCommandCaller)
                return EPermissionResult.Grant;

            GuardLoaded();
            GuardPermission(ref permission);

            if (!permission.StartsWith("!") && HasPermission(caller, "!" + permission) == EPermissionResult.Grant)
                return EPermissionResult.Deny;

            var permissionTree = BuildPermissionTree(permission);
            foreach (var permissionNode in permissionTree)
            {
                string[] playerPermissions = GetConfigSection(caller)["Permissions"].Get(new string[0]);
                if (playerPermissions.Any(c => c.Trim().Equals(permissionNode, StringComparison.OrdinalIgnoreCase)))
                    return EPermissionResult.Grant;
            }

            IEnumerable<IPermissionGroup> groups = GetGroups(caller);
            foreach (var group in groups)
            {
                var result = HasPermission(group, permission);
                if (result == EPermissionResult.Grant)
                    return EPermissionResult.Grant;

                if (result == EPermissionResult.Deny)
                    return EPermissionResult.Deny;
            }
            return EPermissionResult.Default;
        }

        public EPermissionResult HasAllPermissions(IPermissionGroup @group, params string[] permissions)
        {
            GuardLoaded();
            GuardPermissions(permissions);

            EPermissionResult result = EPermissionResult.Grant;

            foreach (var permission in permissions)
            {
                var tmp = HasPermission(@group, permission);
                if (tmp == EPermissionResult.Deny)
                    return EPermissionResult.Deny;

                if (tmp == EPermissionResult.Default)
                    result = EPermissionResult.Default;
            }

            return result;
        }

        public EPermissionResult HasAllPermissions(ICommandCaller caller, params string[] permissions)
        {
            GuardLoaded();
            GuardPermissions(permissions);

            EPermissionResult result = EPermissionResult.Grant;

            foreach (var permission in permissions)
            {
                var tmp = HasPermission(caller, permission);
                if (tmp == EPermissionResult.Deny)
                    return EPermissionResult.Deny;

                if (tmp == EPermissionResult.Default)
                    result = EPermissionResult.Default;
            }

            return result;
        }

        public EPermissionResult HasAnyPermissions(IPermissionGroup @group, params string[] permissions)
        {
            GuardLoaded();
            GuardPermissions(permissions);

            foreach (var permission in permissions)
            {
                var result = HasPermission(@group, permission);
                if (result == EPermissionResult.Deny)
                    return EPermissionResult.Deny;

                if (result == EPermissionResult.Grant)
                    return EPermissionResult.Grant;
            }

            return EPermissionResult.Default;
        }

        public EPermissionResult HasAnyPermissions(ICommandCaller caller, params string[] permissions)
        {
            if (caller is IConsoleCommandCaller)
                return EPermissionResult.Grant;

            GuardLoaded();
            GuardPermissions(permissions);

            foreach (var permission in permissions)
            {
                var result = HasPermission(caller, permission);
                if (result == EPermissionResult.Deny)
                    return EPermissionResult.Deny;

                if (result == EPermissionResult.Grant)
                    return EPermissionResult.Grant;
            }

            return EPermissionResult.Default;
        }

        public bool AddPermission(IPermissionGroup group, string permission)
        {
            GuardPermission(ref permission);
            var permsSection = GetConfigSection(group)["Permissions"];
            List<string> groupPermissions = permsSection.Get(defaultValue: new string[0]).ToList();
            groupPermissions.Add(permission);
            permsSection.Set(groupPermissions.ToArray());
            return true;
        }

        public bool AddInvertedPermission(IPermissionGroup group, string permission)
        {
            GuardPermission(ref permission);
            return AddPermission(group, "!" + permission);
        }

        public bool AddPermission(ICommandCaller caller, string permission)
        {
            if (caller is IConsoleCommandCaller)
                return false;

            GuardPermission(ref permission);
            var permsSection = GetConfigSection(caller)["Permissions"];
            List<string> groupPermissions = permsSection.Get(defaultValue: new string[0]).ToList();
            groupPermissions.Add(permission);
            permsSection.Set(groupPermissions.ToArray());
            return true;
        }
        public bool AddInvertedPermission(ICommandCaller caller, string permission)
        {
            if (caller is IConsoleCommandCaller)
                return false;

            GuardPermission(ref permission);
            return AddPermission(caller, "!" + permission);
        }

        public bool RemovePermission(IPermissionGroup group, string permission)
        {
            GuardPermission(ref permission);
            var permsSection = GetConfigSection(group)["Permissions"];
            List<string> groupPermissions = permsSection.Get(defaultValue: new string[0]).ToList();
            int i = groupPermissions.RemoveAll(c => c.Trim().Equals(permission, StringComparison.OrdinalIgnoreCase));
            permsSection.Set(groupPermissions.ToArray());
            return i > 0;
        }

        public bool RemoveInvertedPermission(IPermissionGroup group, string permission)
        {
            GuardPermission(ref permission);
            return RemovePermission(group, "!" + permission);
        }

        public bool RemovePermission(ICommandCaller caller, string permission)
        {
            if (caller is IConsoleCommandCaller)
                return false;

            GuardPermission(ref permission);
            var permsSection = GetConfigSection(caller)["Permissions"];
            List<string> groupPermissions = permsSection.Get(defaultValue: new string[0]).ToList();
            int i = groupPermissions.RemoveAll(c => c.Trim().Equals(permission, StringComparison.OrdinalIgnoreCase));
            permsSection.Set(groupPermissions.ToArray());
            return i > 0;
        }

        public bool RemoveInvertedPermission(ICommandCaller caller, string permission)
        {
            if (caller is IConsoleCommandCaller)
                return false;

            GuardPermission(ref permission);
            return RemovePermission(caller, "!" + permission);
        }

        public IPermissionGroup GetPrimaryGroup(ICommandCaller caller)
        {
            if (caller is IConsoleCommandCaller)
                return null;

            GuardLoaded();
            return GetGroups(caller).OrderByDescending(c => c.Priority).FirstOrDefault();
        }

        public IEnumerable<IPermissionGroup> GetGroups(ICommandCaller caller)
        {
            if (caller is IConsoleCommandCaller)
                return new PermissionGroup[0];

            GuardLoaded();
            IConfigurationSection groupsSection = GetConfigSection(caller)["Groups"];
            string[] groups = groupsSection.Get(defaultValue: new string[0]);
            return groups
                   .Select(GetGroup)
                   .Where(c => c != null);
        }

        public IPermissionGroup GetGroup(string id)
        {
            return GetGroups().FirstOrDefault(c => c.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<IPermissionGroup> GetGroups()
        {
            GuardLoaded();
            List<IPermissionGroup> groups = new List<IPermissionGroup>();
            foreach (IConfigurationSection child in GroupsConfig)
            {
                PermissionGroup group = new PermissionGroup();
                group.Id = child.Key;
                group.Name = child["Name"].Get(defaultValue: child.Key);
                group.Priority = child.Get(0);
                groups.Add(group);
            }

            return groups;
        }

        public void UpdateGroup(IPermissionGroup @group)
        {
            GuardLoaded();
            if (GetGroup(@group.Id) == null)
                throw new Exception("Can't update group that does not exist: " + @group.Id);

            IConfigurationSection section = GroupsConfig.GetSection($"{@group.Id}");

            if (!section.ChildExists("Name"))
                section.CreateSection("Name", SectionType.Value);
            section["Name"].Set(@group.Name);

            if (!section.ChildExists("Priority"))
                section.CreateSection("Priority", SectionType.Value);
            section["Priority"].Set(@group.Priority);
        }

        public void AddGroup(ICommandCaller caller, IPermissionGroup @group)
        {
            if (caller is IConsoleCommandCaller)
                return;

            GuardLoaded();
            IConfigurationSection groupsSection = GetConfigSection(caller)["Groups"];
            List<string> groups = groupsSection.Get(defaultValue: new string[0]).ToList();
            if (!groups.Any(c => c.Equals(@group.Id, StringComparison.OrdinalIgnoreCase)))
                groups.Add(@group.Id);
            groupsSection.Set(groups.ToArray());
        }

        public bool RemoveGroup(ICommandCaller caller, IPermissionGroup @group)
        {
            if (caller is IConsoleCommandCaller)
                return false;

            GuardLoaded();
            IConfigurationSection groupsSection = GetConfigSection(caller)["Groups"];
            List<string> groups = groupsSection.Get(defaultValue: new string[0]).ToList();
            int i = groups.RemoveAll(c => c.Equals(@group.Id, StringComparison.OrdinalIgnoreCase));
            groupsSection.Set(groups.ToArray());
            return i > 0;
        }

        public void CreateGroup(IPermissionGroup @group)
        {
            GuardLoaded();
            IConfigurationSection section = GroupsConfig.CreateSection($"{@group.Id}", SectionType.Object);
            section.CreateSection("Name", SectionType.Value).Set(@group.Name);
            section.CreateSection("Priority", SectionType.Value).Set(@group.Priority);
        }

        public void DeleteGroup(IPermissionGroup @group)
        {
            GuardLoaded();
            GroupsConfig.RemoveSection($"{@group.Id}");
        }

        public void Load(IConfigurationElement groupsConfig, IConfigurationElement playersConfig)
        {
            GroupsConfig = groupsConfig;
            PlayersConfig = playersConfig;
        }

        public void Reload()
        {
            GroupsConfig.Root?.Reload();
            PlayersConfig.Root?.Reload();
        }

        public void Save()
        {
            GroupsConfig.Root?.Save();
            PlayersConfig.Root?.Save();
        }

        /// <summary>
        /// Builds a permission tree for the given permission <br/>
        /// Will 
        /// <b>Example Input:</b>
        /// <code>
        /// "player.test.sub"
        /// </code>
        /// <b>Example output:</b>
        /// <code>
        /// {
        ///     "*",
        ///     "player.*",
        ///     "player.test.*",
        ///     "player.test.sub"
        /// }
        /// </code>
        /// </summary>
        /// <param name="permission"></param>
        /// <returns>A collection of all permission nodes</returns>
        public IEnumerable<string> BuildPermissionTree(string permission)
        {
            List<string> permissions = new List<string>
            {
                "*"
            };

            string parentPath = "";
            foreach (var childPath in permission.Split('.'))
            {
                permissions.Add(parentPath + childPath + ".*");
                parentPath += childPath + ".";
            }

            //remove last element because it should not contain "<permission>.*"
            //If someone has "permission.x.*" they should not have "permission.x" too
            permissions.RemoveAt(permissions.Count - 1);

            permissions.Add(permission);
            return permissions;
        }

        private void GuardPermission(ref string permission)
        {
            if (string.IsNullOrEmpty(permission))
                throw new ArgumentException("Argument can not be null or empty", nameof(permission));

            //permission = permission.ToLower().Trim();
            permission = permission.Trim();
        }

        private void GuardPermissions(string[] permissions)
        {
            for (int i = 0; i < permissions.Length; i++)
            {
                var tmp = permissions[i];
                GuardPermission(ref tmp);
                permissions[i] = tmp;
            }
        }

        private IConfigurationSection GetConfigSection(IPermissionGroup group)
        {
            var config = GroupsConfig;

            var basePath = $"{group.Id}";
            string permissionsPath = basePath + ".Permissions";

            if (!config.ChildExists(permissionsPath))
            {
                config.CreateSection(permissionsPath, SectionType.Array);
                config[permissionsPath].Set(new string[0]);
            }

            return config[basePath];
        }

        private IConfigurationSection GetConfigSection(ICommandCaller caller)
        {
            var config = PlayersConfig;
            var basePath = $"{caller.PlayerType.Name}.{caller.Id}";
            string permissionsPath = basePath + ".Permissions";
            string groupsPath = basePath + ".Groups";

            if (!config.ChildExists(permissionsPath))
            {
                config.CreateSection(permissionsPath, SectionType.Array);
                config[permissionsPath].Set(new string[0]);
            }

            if (!config.ChildExists(groupsPath))
            {
                config.CreateSection(groupsPath, SectionType.Array);
                config[groupsPath].Set(new string[0]);
            }

            return config[basePath];
        }

        private void GuardLoaded()
        {
            if (GroupsConfig == null || (GroupsConfig.Root != null && !GroupsConfig.Root.IsLoaded))
                throw new Exception("Groups config not loaded!");

            if (PlayersConfig == null || (PlayersConfig.Root != null && !PlayersConfig.Root.IsLoaded))
                throw new Exception("Players config has not been loaded");
        }
    }
}