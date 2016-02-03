using Rocket.Core.Assets;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Rocket.API.Serialisation
{
    [Serializable]
    public class RocketPermissions : IDefaultable
    {
        public RocketPermissions()
        {
        }

        [XmlElement("DefaultGroupId")]
        public string DefaultGroupId;

        [XmlArray("Groups")]
        [XmlArrayItem(ElementName = "Group")]
        public RocketPermissionsGroup[] Groups;

        public void LoadDefaults()
        {
            DefaultGroupId = "default";
            Groups = new RocketPermissionsGroup[] {
                new RocketPermissionsGroup("default","Guest",null, new List<string>() , new List<Permission>() { new Permission("p"), new Permission("reward"), new Permission("balance"), new Permission("pay"), new Permission("rocket")},"white"),
                new RocketPermissionsGroup("vip","VIP", "default",new List<string>() { "76561198016438091" }, new List<Permission>() { new Permission("v",30) },"FF9900")
            };
        }
    }
}
