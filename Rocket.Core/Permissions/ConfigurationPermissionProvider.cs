using Rocket.API.Configuration;
using Rocket.API.Permissions;
using Rocket.API.User;
using Rocket.Core.Configuration;
using Rocket.Core.ServiceProxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocket.Core.Permissions
{
    [ServicePriority(Priority = ServicePriority.Low)]
    public class ConfigurationPermissionProvider : IPermissionProvider, IPermissionChecker
    {
        public ConfigurationPermissionProvider(IConfiguration groupsConfig,
                                               IConfiguration playersConfig)
        {
            GroupsConfig = groupsConfig;
            PlayersConfig = playersConfig;
        }

        public IConfiguration GroupsConfig { get; protected set; }
        public IConfiguration PlayersConfig { get; protected set; }

        public async Task<IEnumerable<string>> GetGrantedPermissionsAsync(IPermissionActor target, bool inherit = true)
        {
            PermissionSection section = target is IPermissionGroup
                ? (PermissionSection)await GetConfigSectionAsync<GroupPermissionSection>(target, false)
                : await GetConfigSectionAsync<PlayerPermissionSection>(target, false);

            List<string> permissions;
            if (section != null)
            {
                permissions = section?.Permissions
                                     .Where(c => !c.StartsWith("!"))
                                     .Select(c => c.ToLower())
                                     .ToList();
            }
            else
            {
                permissions = new List<string>();
            }

            if (inherit)
            {
                foreach (var parent in await GetGroupsAsync(target))
                    permissions
                        .AddRange(await GetGrantedPermissionsAsync(parent, true));
            }

            return permissions.Distinct();
        }

        public async Task<IEnumerable<string>> GetDeniedPermissionsAsync(IPermissionActor target, bool inherit = true)
        {
            PermissionSection section = target is IPermissionGroup
                ? (PermissionSection)await GetConfigSectionAsync<GroupPermissionSection>(target, false)
                : await GetConfigSectionAsync<PlayerPermissionSection>(target, false);

            List<string> permissions;
            if (section != null)
            {
                permissions = section.Permissions
                                     .Where(c => c.StartsWith("!"))
                                     .Select(c => c.Substring(1).ToLower())
                                     .ToList();
            }
            else
            {
                permissions = new List<string>();
            }

            if (inherit)
            {
                foreach (var parent in await GetGroupsAsync(target))
                    permissions
                        .AddRange(await GetDeniedPermissionsAsync(parent, true));
            }

            return permissions.Distinct();
        }

        public bool SupportsTarget(IPermissionActor target)
            => target is IPermissionGroup || target is IUser;

        public async Task<PermissionResult> CheckPermissionAsync(IPermissionActor target, string permission)
        {
            GuardLoaded();
            GuardPermission(ref permission);
            GuardTarget(target);

            permission = permission.Replace("!!", ""); //remove double negations

            if (!permission.StartsWith("!") && await CheckPermissionAsync(target, "!" + permission) == PermissionResult.Grant)
                return PermissionResult.Deny;

            IEnumerable<string> permissionTree = BuildPermissionTree(permission);

            PermissionSection section = target is IPermissionGroup
                ? (PermissionSection) await GetConfigSectionAsync<GroupPermissionSection>(target, false)
                : await GetConfigSectionAsync<PlayerPermissionSection>(target, false);

            string[] permissions = section?.Permissions ?? new string[0];

            foreach (string permissionNode in permissionTree)
            {
                foreach (string c in permissions)
                {
                    if (c.Contains("*") && !c.StartsWith("!") && permission.StartsWith("!"))
                        continue;

                    if (c.Trim().Equals(permissionNode, StringComparison.OrdinalIgnoreCase))
                    {
                        return PermissionResult.Grant;
                    }
                }
            }

            // check parent group permissions / player group permissions
            IEnumerable<IPermissionGroup> groups = await GetGroupsAsync(target);
            foreach (IPermissionGroup group in groups)
            {
                PermissionResult result = await CheckPermissionAsync(group, permission);
                if (result == PermissionResult.Grant)
                    return PermissionResult.Grant;

                if (result == PermissionResult.Deny)
                    return PermissionResult.Deny;
            }

            return PermissionResult.Default;
        }

        public async Task<bool> AddPermissionAsync(IPermissionActor target, string permission)
        {
            GuardPermission(ref permission);
            GuardTarget(target);

            PermissionSection section = (target is IPermissionGroup
                ? (PermissionSection)await GetConfigSectionAsync<GroupPermissionSection>(target, true)
                : await GetConfigSectionAsync<PlayerPermissionSection>(target, true));

            List<string> permissions = section.Permissions.ToList();
            permissions.Add(permission);
            section.Permissions = permissions.ToArray();
            await section.SaveAsync();
            return true;
        }

        public Task<bool> AddDeniedPermissionAsync(IPermissionActor target, string permission)
        {
            GuardPermission(ref permission);
            GuardTarget(target);

            return AddPermissionAsync(target, "!" + permission);
        }

        public async Task<bool> RemovePermissionAsync(IPermissionActor target, string permission)
        {
            GuardPermission(ref permission);

            PermissionSection section = (target is IPermissionGroup
                ? (PermissionSection)await GetConfigSectionAsync<GroupPermissionSection>(target, false)
                : await GetConfigSectionAsync<PlayerPermissionSection>(target, false));

            if (section == null)
                return false;

            List<string> permissions = section.Permissions.ToList();
            int i = permissions.RemoveAll(c => c.Trim().Equals(permission, StringComparison.OrdinalIgnoreCase));
            section.Permissions = permissions.ToArray();
            await section.SaveAsync();
            return i > 0;
        }

        public async Task<bool> RemoveDeniedPermissionAsync(IPermissionActor target, string permission)
        {
            GuardPermission(ref permission);
            GuardTarget(target);

            return await RemovePermissionAsync(target, "!" + permission);
        }

        public async Task<IPermissionGroup> GetPrimaryGroupAsync(IPermissionActor permissionActor)
        {
            GuardLoaded();
            if (!(permissionActor is IUser))
                throw new NotSupportedException();

            return (await GetGroupsAsync(permissionActor)).OrderByDescending(c => c.Priority).FirstOrDefault();
        }

        public async Task<IEnumerable<IPermissionGroup>> GetGroupsAsync(IPermissionActor target)
        {
            GuardLoaded();
            GuardTarget(target);

            PermissionSection section =(target is IPermissionGroup
                ? (PermissionSection) await GetConfigSectionAsync<GroupPermissionSection>(target, false)
                : await GetConfigSectionAsync<PlayerPermissionSection>(target, false));

            if (section == null)
            {
                return (await GetGroupsAsync()).Where(c => c.AutoAssign);
            }

            return section.GetGroups()
                          .Select(e => GetGroupAsync(e).GetAwaiter().GetResult())
                          .Where(c => c != null);
        }

        public async Task<IPermissionGroup> GetGroupAsync(string id)
        {
            return (await GetGroupsAsync()).FirstOrDefault(c => c.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<IEnumerable<IPermissionGroup>> GetGroupsAsync()
        {
            GuardLoaded();
            return GroupsConfig["Groups"]
                   .Get<GroupPermissionSection[]>()
                   .Select(c => new PermissionGroup
                   {
                       Id = c.Id,
                       Name = c.Name,
                       Priority = c.Priority,
                       AutoAssign = c.AutoAssign
                   })
                   .Cast<IPermissionGroup>()
                   .ToList();
        }

        public async Task<bool> UpdateGroupAsync(IPermissionGroup group)
        {
            GuardLoaded();
            GuardTarget(group);

            GroupPermissionSection section = await GetConfigSectionAsync<GroupPermissionSection>(group, false);
            if (section == null)
                return false;

            section.Name = group.Name;
            section.Priority = group.Priority;
            await section.SaveAsync();
            return true;
        }

        public async Task<bool> AddGroupAsync(IPermissionActor target, IPermissionGroup group)
        {
            GuardLoaded();
            GuardTarget(target);
            GuardTarget(group);

            PermissionSection section = target is IPermissionGroup
                ? (PermissionSection) await GetConfigSectionAsync<GroupPermissionSection>(target, true)
                : await GetConfigSectionAsync<PlayerPermissionSection>(target, true);

            List<string> groups = section.GetGroups().ToList();
            if (!groups.Any(c => c.Equals(group.Id, StringComparison.OrdinalIgnoreCase)))
                groups.Add(group.Id);
            section.SetGroups(groups.ToArray());
            await section.SaveAsync();
            return true;
        }

        public async Task<bool> RemoveGroupAsync(IPermissionActor target, IPermissionGroup group)
        {
            GuardLoaded();
            GuardTarget(target);
            GuardTarget(group);

            PermissionSection section = target is IPermissionGroup
                ? (PermissionSection)await GetConfigSectionAsync<GroupPermissionSection>(target, false)
                : await GetConfigSectionAsync<PlayerPermissionSection>(target, false);

            if (section == null)
                return false;

            List<string> groups = section.GetGroups().ToList();
            int i = groups.RemoveAll(c => c.Equals(group.Id, StringComparison.OrdinalIgnoreCase));
            section.SetGroups(groups.ToArray());
            await section.SaveAsync();
            return i > 0;
        }

        public async Task<bool> CreateGroupAsync(IPermissionGroup group)
        {
            GuardLoaded();
            GuardTarget(group);

            GroupPermissionSection section = await GetConfigSectionAsync<GroupPermissionSection>(group, true);
            section.Name = group.Name;
            section.Priority = group.Priority;
            await section.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteGroupAsync(IPermissionGroup group)
        {
            GuardLoaded();
            GuardTarget(group);
            return DeleteConfigSection(group);
        }

        public async Task LoadAsync(IConfigurationContext context)
        {
            IConfigurationContext permissionsContext = context.CreateChildConfigurationContext("Permissions");
            IConfigurationContext groupsContext = permissionsContext.CreateChildConfigurationContext("Groups");
            GroupsConfig.ConfigurationContext = groupsContext;
            await GroupsConfig.LoadAsync(new
            {
                Groups = new object[]
                {
                    new GroupPermissionSection
                    {
                        Id = "Default",
                        Name = "Default Group",
                        Priority = 0,
                        Permissions = new[]
                        {
                            "SomePermission",
                            "SomePermission.Other"
                        },
                        AutoAssign = true
                    }
                }
            });

            IConfigurationContext playersContext = permissionsContext.CreateChildConfigurationContext("Players");
            PlayersConfig.ConfigurationContext = playersContext;
            await PlayersConfig.LoadAsync();
        }

        public async Task ReloadAsync()
        {
            if (GroupsConfig != null)
                await GroupsConfig.Root.ReloadAsync();

            if (PlayersConfig != null)
                await PlayersConfig.Root.ReloadAsync();
        }

        public async Task SaveAsync()
        {
            if (GroupsConfig != null)
                await GroupsConfig.Root.SaveAsync();

            if (PlayersConfig != null)
                await PlayersConfig.Root.SaveAsync();
        }


        /// <summary>
        ///     Builds a parent permission tree for the given permission <br />
        ///     If the target has any of these permissions, they will automatically have the given permission too <br /><br />
        ///     <b>Example Input:</b>
        ///     <code>
        /// "player.test.sub"
        /// </code>
        ///     <b>Example output:</b>
        ///     <code>
        /// [
        ///     "*",
        ///     "player.*",
        ///     "player.test.*",
        ///     "player.test.sub"
        /// ]
        /// </code>
        /// </summary>
        /// <param name="permission">The permission to build the tree for</param>
        /// <returns>The collection of all parent permission nodes</returns>
        public static IEnumerable<string> BuildPermissionTree(string permission)
        {
            List<string> permissions = new List<string>
            {
                "*"
            };

            string parentPath = "";
            foreach (string childPath in permission.Split('.'))
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
                string tmp = permissions[i];
                GuardPermission(ref tmp);
                permissions[i] = tmp;
            }
        }

        private bool DeleteConfigSection(IPermissionActor target)
        {
            IConfigurationElement config = target is IPermissionGroup
                ? GroupsConfig["Groups"]
                : PlayersConfig[((IUser)target).UserType];

            List<PermissionSection> values = config.Get<PermissionSection[]>().ToList();
            int i = values.RemoveAll(c => c.Id.Equals(target.Id, StringComparison.OrdinalIgnoreCase));
            config.Set(values);
            return i > 0;
        }

        public async Task<T> GetConfigSectionAsync<T>(IPermissionActor target, bool createIfNotFound) where T : PermissionSection
        {
            GuardTarget(target);

            bool isPermissionGroup = target is IPermissionGroup;

            IConfiguration config = null;
            string path = null;

            if (isPermissionGroup)
            {
                config = GroupsConfig;
                path = "Groups";
            }
            else
            {
                config = PlayersConfig;
                path = "Users";
            }

            if (createIfNotFound && !config.ChildExists(path))
            {
                config.CreateSection(path, SectionType.Array);
                await config.SaveAsync();
            }
            else if (!createIfNotFound && !config.ChildExists(path))
            {
                return null;
            }

            IConfigurationElement configElement = config[path];

            if (configElement.Type != SectionType.Array)
                throw new Exception("Expected array type but got " + configElement.Type);

            List<PermissionSection> values = configElement.Get<PermissionSection[]>().ToList();
            if (!values.Any(c => c.Id.Equals(target.Id)))
            {
                if (!createIfNotFound)
                    return null;

                PermissionSection toCreate;
                if (target is IPermissionGroup)
                    toCreate = new GroupPermissionSection(target.Id, configElement);
                else
                    toCreate = new PlayerPermissionSection(target.Id, configElement);

                await toCreate.SaveAsync();
            }

            T section = configElement.Get<T[]>().FirstOrDefault(c => c.Id == target.Id);
            section?.SetConfigElement(configElement);
            return section;
        }

        private void GuardLoaded()
        {
            if (GroupsConfig == null || GroupsConfig.Root != null && !GroupsConfig.Root.IsLoaded)
                throw new Exception("Groups config not loaded!");

            if (PlayersConfig == null || PlayersConfig.Root != null && !PlayersConfig.Root.IsLoaded)
                throw new Exception("Players config has not been loaded");
        }

        private void GuardTarget(IPermissionActor target)
        {
            if (!SupportsTarget(target))
                throw new NotSupportedException(target.GetType().FullName + " is not supported!");
        }

        public void LoadFromConfig(IConfiguration groupsConfig, IConfiguration playersConfig)
        {
            GroupsConfig = groupsConfig;
            PlayersConfig = playersConfig;
        }

        public string ServiceName => "RocketPermissions";
    }
}
