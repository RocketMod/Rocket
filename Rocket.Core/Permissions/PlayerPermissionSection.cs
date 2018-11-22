using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        [ConfigArray("Permission")]
        public override string[] Permissions { get; set; } = new string[0];

        [ConfigArray("Group")]
        public string[] Groups { get; set; } = new string[0];

        public override async Task SaveAsync()
        {
            List<PlayerPermissionSection> players = element.Get<PlayerPermissionSection[]>().ToList();
            players.RemoveAll(c => c.Id.Equals(Id, StringComparison.OrdinalIgnoreCase));
            players.Add(this);
            element.Set(players.ToArray());

            if (element.Root.ConfigurationContext != null)
                await element.Root.SaveAsync();
        }

        public override string[] GetGroups() => Groups;

        public override void SetGroups(string[] groups) => Groups = groups;
    }
}