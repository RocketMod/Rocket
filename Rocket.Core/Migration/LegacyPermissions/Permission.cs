using System;
using System.Xml.Serialization;

namespace Rocket.Core.Migration.LegacyPermissions
{
    [Serializable]
    public class Permission
    {
        [XmlAttribute]
        public uint Cooldown = 0;

        [XmlText]
        public string Name = "";
    }
}