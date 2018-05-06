using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Rocket.Core.Migration.LegacyPermissions
{
    [Serializable]
    public class RocketPermissions
    {
        [XmlElement("DefaultGroup")]
        public string DefaultGroup = "default";

        [XmlArray("Groups")]
        [XmlArrayItem(ElementName = "Group")]
        public List<RocketPermissionsGroup> Groups = new List<RocketPermissionsGroup>();
    }
}