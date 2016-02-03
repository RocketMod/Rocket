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

        public override bool Equals(object obj)
        {
            return Name.ToLower().Equals(obj.ToString().ToLower());
        }

        public override string ToString()
        {
            return Name;
        }

        public static implicit operator Permission(string value)
        {
            return new Permission { Name = value };
        }

        public static bool operator ==(Permission p1, Permission p2)
        {
            return p1.Equals(p2);
        }

        public static bool operator !=(Permission p1, Permission p2)
        {
            return !p1.Equals(p2);
        }

    }
}
