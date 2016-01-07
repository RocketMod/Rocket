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
                new RocketPermissionsGroup("default","Guest",new List<string>(), new List<string>() , new List<string>() { "p", "reward","balance","pay","rocket","color.white" }),
                new RocketPermissionsGroup("vip","VIP", new List<string>(),new List<string>() { "76561198016438091" }, new List<string>() { "color.FF9900" })
            };
        }
    }
}
