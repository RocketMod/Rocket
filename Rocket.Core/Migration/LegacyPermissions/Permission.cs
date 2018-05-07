using System;
using System.Xml.Serialization;

namespace Rocket.Core.Migration.LegacyPermissions
{
    public class Permission
    {
        public uint Cooldown { get; set; }

        public string Name { get; set; }
    }
}