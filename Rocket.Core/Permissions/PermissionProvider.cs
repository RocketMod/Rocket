using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.Configuration;
using Rocket.API.Permissions;

namespace Rocket.Core.Permissions
{
    public class PermissionProvider : IPermissionProvider
    {
        public IConfiguration GroupsConfig { get; protected set; }
        public IConfiguration PlayersConfig { get; protected set; }

        public bool HasPermission(IPermissionGroup @group, string permission)
        {
            GuardLoaded();

            string[] groupPermissions = GroupsConfig.GetSection("Groups." + group.Id + ".Permissions").Get<string[]>();
            return groupPermissions.Any(c => c.Trim().Equals(permission.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public bool HasPermission(ICommandCaller caller, string permission)
        {
            GuardLoaded();
            //todo: force remove permissions with ! prefix
            IEnumerable<IPermissionGroup> groups = GetGroups(caller);
            return groups.Any(c => HasPermission(c, permission));
        }

        public bool HasAllPermissions(IPermissionGroup @group, params string[] permissions)
        {
            GuardLoaded();
            return permissions.All(c => HasPermission(group, c));
        }

        public bool HasAllPermissions(ICommandCaller caller, params string[] permissions)
        {
            GuardLoaded();
            return permissions.All(c => HasPermission(caller, c));
        }

        public bool HasAnyPermissions(IPermissionGroup @group, params string[] permissions)
        {
            GuardLoaded();
            return permissions.Any(c => HasPermission(group, c));
        }

        public bool HasAnyPermissions(ICommandCaller caller, params string[] permissions)
        {
            GuardLoaded();
            return permissions.Any(c => HasPermission(caller, c));
        }

        public IPermissionGroup GetPrimaryGroup(ICommandCaller caller)
        {
            GuardLoaded();
            return GetGroups(caller).OrderByDescending(c => c.Priority).FirstOrDefault();
        }

        public IEnumerable<IPermissionGroup> GetGroups(ICommandCaller caller)
        {
            GuardLoaded();
            IConfigurationSection groupsSection = PlayersConfig.GetSection($"Players.{caller.PlayerType.Name}.{caller.Id}.Groups");
            string[] groups = groupsSection.Get<string[]>();
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
            IConfigurationSection section = GroupsConfig.GetSection("Groups");

            List<IPermissionGroup> groups = new List<IPermissionGroup>();
            foreach (IConfigurationSection child in section)
            {
                PermissionGroup group = new PermissionGroup();
                group.Id = child.Key;
                group.Name = child["Name"].Get<string>();
                group.Priority = child.Get(0);
                groups.Add(group);
            }

            return groups;
        }

        public void UpdateGroup(IPermissionGroup @group)
        {
            GuardLoaded();
            IConfigurationSection section = GroupsConfig.GetSection($"Groups.{@group.Id}");
            section["Name"].Set(@group.Name);
            section.GetSection("Priority").Set(@group.Priority);
        }

        public void AddGroup(ICommandCaller caller, IPermissionGroup @group)
        {
            GuardLoaded();
            throw new NotImplementedException();
        }

        public void RemoveGroup(ICommandCaller caller, IPermissionGroup @group)
        {
            GuardLoaded();
            throw new NotImplementedException();
        }

        public void CreateGroup(IPermissionGroup @group)
        {
            GuardLoaded();
            IConfigurationSection section = GroupsConfig.CreateSection($"Groups.{@group.Id}");
            section["Name"].Set(@group.Name);
            section.GetSection("Priority").Set(@group.Priority);
        }

        public void DeleteGroup(IPermissionGroup @group)
        {
            GuardLoaded();
            GroupsConfig.RemoveSection($"Groups.{@group.Id}");
        }

        public void Load(IConfiguration groupsConfig, IConfiguration playersConfig)
        {
            GroupsConfig = groupsConfig;
            PlayersConfig = playersConfig;
        }

        public void Reload()
        {
            GroupsConfig.Reload();
        }

        public void Save()
        {
            GroupsConfig.Save();
        }

        private void GuardLoaded()
        {
            if (GroupsConfig == null || !GroupsConfig.IsLoaded)
                throw new Exception("Groups config not loaded!");

            if (PlayersConfig == null || !PlayersConfig.IsLoaded)
                throw new Exception("Players config has not been loaded");
        }
    }
}