using Rocket.Core.Configuration;

namespace Rocket.Core.Migration.LegacyPermissions
{
    public class RocketPermissions
    {
        public string DefaultGroup { get; set; }

        [ConfigArray(ElementName = "Group")]
        public RocketPermissionsGroup[] Groups { get; set; }
    }
}