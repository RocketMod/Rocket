using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Rocket.Core.Migration.LegacyPermissions
{
    [Serializable]
    public class RocketPermissionsGroup
    {
        [XmlElement("Id")]
        public string Id;

        [XmlElement("DisplayName")]
        public string DisplayName;

        [XmlElement("Color")]
        public string Color = "white";

        [XmlArray("Members")]
        [XmlArrayItem(ElementName = "Member")]
        public List<string> Members;

        [XmlArray("Permissions")]
        [XmlArrayItem(ElementName = "Permission")]
        public List<Permission> Permissions;

        [XmlElement("ParentGroup")]
        public string ParentGroup;
    }
}