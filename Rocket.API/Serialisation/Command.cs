using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Rocket.API.Serialisation
{
    [Serializable]
    public class Permission
    {
        [XmlAttribute]
        public uint? Cooldown = null;

        [XmlText]
        public string Name = "";

        public Permission() { }

        public Permission(string name, uint? cooldown = null)
        {
            Name = name;
            Cooldown = cooldown;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
