using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.API.Collections;

namespace Rocket.API.Serialization
{
    public enum BuiltinProperties
    {
        COLOR = 0,
        PREFIX = 1,
        SUFFIX = 2
    }

    [Serializable]
    public class RocketPermissionsGroup
    {
        public RocketPermissionsGroup()
        {
        }

        public RocketPermissionsGroup(string id, string displayName, string parentGroup, List<string> members, List<string> permissions, PropertyList properties)
        {
            Id = id;
            DisplayName = displayName;
            Members = members;
            Permissions = permissions;
            ParentGroup = parentGroup;
            Properties = properties;
        }

        [XmlElement("Id")]
        public string Id { get; private set; } = "";

        [XmlElement("DisplayName")]
        public string DisplayName { get; set; } = "";

        [XmlArray("Properties")]
        [XmlArrayItem("Property")]
        public PropertyList Properties { get; set; } = new PropertyList()
        {
            { BuiltinProperties.COLOR , "#FFFFFF" },
            { BuiltinProperties.PREFIX , "[" },
            { BuiltinProperties.SUFFIX , "]" }
        };
        
        [XmlArray("Members")]
        [XmlArrayItem(ElementName = "Member")]
        public List<string> Members { get; set; } = new List<string>();

        [XmlArray("Permissions")]
        [XmlArrayItem(ElementName = "Permission")]
        public List<string> Permissions { get; set; } = new List<string>();

        [XmlElement("ParentGroup")]
        public string ParentGroup { get; set; } = "";

        [XmlElement("Priority")]
        public uint Priority { get; set; } = 0;

    }
}