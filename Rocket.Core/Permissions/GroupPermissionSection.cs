using System;
using System.Linq;
using Rocket.API.Configuration;
using Rocket.Core.Configuration;

namespace Rocket.Core.Permissions
{
    public class GroupPermissionSection : PermissionSection
    {
        public override string Id { get; set; }

        public string Name { get; set; }

        [ConfigArray(ElementName = "Permission")]
        public override string[] Permissions { get; set; } = new string[0];

        [ConfigArray(ElementName = "Group")]
        public string[] ParentGroups { get; set; } = new string[0];

        public int Priority { get; set; }

        public bool AutoAssign { get; set; } = false;

        public GroupPermissionSection(string id, IConfigurationElement element) : base(id, element)
        {
            ParentGroups = new string[0];
            Priority = 0;
        }

        public GroupPermissionSection()
        {

        }

        public override void Save()
        {
            var groups = element.Get<GroupPermissionSection[]>().ToList();
            groups.RemoveAll(c => c.Id.Equals(Id, StringComparison.OrdinalIgnoreCase));
            groups.Add(this);
            element.Set(groups.ToArray());
        }

        public override string[] GetGroups() => ParentGroups;

        public override void SetGroups(string[] groups) => ParentGroups = groups;
    }
}