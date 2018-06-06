using System;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.Configuration;
using Rocket.Core.Configuration;

namespace Rocket.Core.Permissions
{
    public class PlayerPermissionSection : PermissionSection
    {
        public PlayerPermissionSection(string id, IConfigurationElement element) : base(id, element)
        {
            Groups = new string[0];
        }

        public PlayerPermissionSection() { }

        public override string Id { get; set; }

        [ConfigArray(ElementName = "Permission")]
        public override string[] Permissions { get; set; } = new string[0];

        [ConfigArray(ElementName = "Group")]
        public string[] Groups { get; set; } = new string[0];

        public override void Save()
        {
            List<PlayerPermissionSection> players = element.Get<PlayerPermissionSection[]>().ToList();
            players.RemoveAll(c => c.Id.Equals(Id, StringComparison.OrdinalIgnoreCase));
            players.Add(this);
            element.Set(players.ToArray());

            if (element.Root.ConfigurationContext != null)
                element.Root.Save();
        }

        public override string[] GetGroups() => Groups;

        public override void SetGroups(string[] groups) => Groups = groups;
    }
}