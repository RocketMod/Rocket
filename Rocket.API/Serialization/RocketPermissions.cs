using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Rocket.API.Collections;

namespace Rocket.API.Serialization
{
    [Serializable]
    public class RocketPermissions : IDefaultable
    {
        public RocketPermissions()
        {
        }

        [XmlElement("DefaultGroup")]
        public string DefaultGroup = "default";

        [XmlArray("Groups")]
        [XmlArrayItem(ElementName = "Group")]
        public List<RocketPermissionsGroup> Groups = new List<RocketPermissionsGroup>();

        public void LoadDefaults()
        {
            DefaultGroup = "default";
            Groups = new List<RocketPermissionsGroup> {
                new RocketPermissionsGroup("default","Guest",null, new List<string>() , new List<string>() { "p", "compass",  "rocket"},new PropertyList()),
                new RocketPermissionsGroup("vip","VIP", "default",new List<string>() { "76561198016438091" }, new List<string>() {  "effect", "heal", "v" },new PropertyList())
            };
        }
    }
}