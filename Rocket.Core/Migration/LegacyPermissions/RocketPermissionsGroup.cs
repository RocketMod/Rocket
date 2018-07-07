using Rocket.Core.Configuration;

namespace Rocket.Core.Migration.LegacyPermissions
{
    public class RocketPermissionsGroup
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Color { get; set; }

        [ConfigArray("Member")]
        public string[] Members { get; set; }

        [ConfigArray("Permission")]
        public Permission[] Permissions { get; set; }

        public string ParentGroup { get; set; }
    }
}