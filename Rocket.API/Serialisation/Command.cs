using System;
using System.Xml.Serialization;

namespace Rocket.API.Serialisation
{
    [Serializable]
    public class Permission
    {

        [XmlText]
        public string Name { get; set; } = "";

        public Permission()
        {
        }

        public Permission(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}