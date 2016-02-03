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

        public RocketPermissionsGroup(string id, string displayName, string parentGroup, List<string> members, List<Permission> commands,string color = null)
        {
            Id = id;
            DisplayName = displayName;
            Members = members;
            Commands = commands;
            ParentGroup = parentGroup;
            Color = color;
        }

        [XmlElement("Id")]
        public string Id;

        [XmlElement("DisplayName")]
        public string DisplayName;

        [XmlElement("Color")]
        public string Color = "white";

        [XmlArray("Members")]
        [XmlArrayItem(ElementName = "Member")]
        public List<string> Members;

        [XmlArray("Commands")]
        [XmlArrayItem(ElementName = "Command")]
        public List<Permission> Commands;

        [XmlElement("ParentGroup")]
        public string ParentGroup;
    }
}
