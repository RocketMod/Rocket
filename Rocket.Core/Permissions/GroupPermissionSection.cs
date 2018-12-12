using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rocket.API.Configuration;
using Rocket.Core.Configuration;

namespace Rocket.Core.Permissions
{
    public class GroupPermissionSection : PermissionSection
    {
        public GroupPermissionSection(string id, IConfigurationElement element) : base(id, element)
        {
            ParentGroups = new string[0];
            Priority = 0;
        }

        public GroupPermissionSection() { }

        public override string Id { get; set; }

        public string Name { get; set; }

        [ConfigArray("Permission")]
        public override string[] Permissions { get; set; } = new string[0];

        [ConfigArray("Group")]
        public string[] ParentGroups { get; set; } = new string[0];

        public int Priority { get; set; }

        public bool AutoAssign { get; set; } = false;

        public override async Task SaveAsync()
        {
            List<GroupPermissionSection> groups = element.Get<GroupPermissionSection[]>().ToList();
            groups.RemoveAll(c => c.Id.Equals(Id, StringComparison.OrdinalIgnoreCase));
            groups.Add(this);
            element.Set(groups.ToArray());

            if (element.Root.ConfigurationContext != null)
                await element.Root.SaveAsync();
        }

        public override string[] GetGroups() => ParentGroups;

        public override void SetGroups(string[] groups) => ParentGroups = groups;
    }
}