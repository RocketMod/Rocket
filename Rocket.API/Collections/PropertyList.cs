using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Rocket.API.Collections
{
    [Serializable]
    [XmlType(AnonymousType = false, IncludeInSchema = true)]
    public class PropertyListEntry
    {
        [XmlAttribute]
        public string Key;

        [XmlAttribute]
        public string Value;

        public PropertyListEntry(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public PropertyListEntry()
        {
        }
    }

    [Serializable]
    public class PropertyList : IEnumerable<PropertyListEntry>
    {
        public PropertyList()
        {

        }

        protected List<PropertyListEntry> properties = new List<PropertyListEntry>();

        public IEnumerator<PropertyListEntry> GetEnumerator()
        {
            return properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return properties.GetEnumerator();
        }

        public void Add(Object o)
        {
            properties.Add((PropertyListEntry)o);
        }

        public void Add(string key, string value)
        {
            properties.Add(new PropertyListEntry(key, value));
        }

        public void Add(Enum key, string value)
        {
            properties.Add(new PropertyListEntry(key.ToString(), value));
        }

        public void AddRange(IEnumerable<PropertyListEntry> collection)
        {
            properties.AddRange(collection);
        }

        public string this[Enum key]
        {
            get
            {
                return properties.Where(k => k.Key == key.ToString()).Select(k => k.Value).FirstOrDefault();
            }
            set
            {
                properties.ForEach(k => { if (k.Value == key.ToString()) k.Value = value; });
            }
        }

        public string this[string key]
        {
            get
            {
                return properties.Where(k => k.Key == key).Select(k => k.Value).FirstOrDefault();
            }
            set
            {
                properties.ForEach(k => { if (k.Value == key) k.Value = value; });
            }
        }
    }
}
