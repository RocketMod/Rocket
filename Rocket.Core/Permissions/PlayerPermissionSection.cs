using System;
using System.Linq;
using Rocket.API.Configuration;

namespace Rocket.Core.Permissions {
    public class PlayerPermissionSection : PermissionSection
    {
        public override string Id { get; set; }

        public override string[] Permissions { get; set; } = new string[0];

        public string[] Groups { get; set; } = new string[0];

        public PlayerPermissionSection(string id, IConfigurationElement element) : base(id, element)
        {
            Groups = new string[0];
        }

        public PlayerPermissionSection()
        {

        }

        public override void Save()
        {
            var players = element.Get<PlayerPermissionSection[]>().ToList();
            players.RemoveAll(c => c.Id.Equals(Id, StringComparison.OrdinalIgnoreCase));
            players.Add(this);
            element.Set(players.ToArray());
        }

        public override string[] GetGroups() => Groups;

        public override void SetGroups(string[] groups) => Groups = groups;
    }
}