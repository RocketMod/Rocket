using Rocket.Core.Configuration;

namespace Rocket.Core.Migration.LegacyPermissions
{
    public class RocketPermissions
    {
        public string DefaultGroup { get; set; }

        [ConfigArray("Group")]
        public RocketPermissionsGroup[] Groups { get; set; }
    }
}