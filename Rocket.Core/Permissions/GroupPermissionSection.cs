using System;
using System.Linq;
using Rocket.API.Configuration;
using Rocket.API.Permissions;
using Rocket.Core.Configuration;

namespace Rocket.Core.Permissions
{
    public class GroupPermissionSection : PermissionSection, IPermissionGroup
    {
        public override string Id { get; set; }

        public string Name { get; set; }

        [ConfigArray(ElementName = "Permission")]
        public override string[] Permissions { get; set; } = new string[0];

        [ConfigArray(ElementName = "Group")]
        public string[] ParentGroups { get; set; } = new string[0];

        public int Priority { get; set; } = 0;

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
        public int CompareTo(object obj)
        {
            throw new System.NotImplementedException();
        }

        public int CompareTo(IIdentity other)
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(IIdentity other)
        {
            throw new System.NotImplementedException();
        }

        public int CompareTo(string other)
        {
            throw new System.NotImplementedException();
        }

        public bool Equals(string other)
        {
            throw new System.NotImplementedException();
        }
    }
}