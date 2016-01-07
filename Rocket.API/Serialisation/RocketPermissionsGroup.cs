using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Rocket.API.Serialisation
{
    [Serializable]
    public class RocketPermissionsGroup
    {
        public RocketPermissionsGroup()
        {
        }

        public RocketPermissionsGroup(string id, string displayName, List<string> parentGroups, List<string> members, List<string> commands)
        {
            Id = id;
            DisplayName = displayName;
            Members = members;
            Commands = commands;
            ParentGroups = parentGroups;
        }

        [XmlElement("Id")]
        public string Id;

        [XmlElement("DisplayName")]
        public string DisplayName;

        [XmlArray("Members")]
        [XmlArrayItem(ElementName = "Member")]
        public List<string> Members;

        [XmlArray("Commands")]
        [XmlArrayItem(ElementName = "Command")]
        public List<string> Commands;

        [XmlArray("ParentGroups")]
        [XmlArrayItem(ElementName = "ParentGroup")]
        public List<string> ParentGroups;
    }
}
