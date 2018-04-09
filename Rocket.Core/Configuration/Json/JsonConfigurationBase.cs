using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration.Json
{
    public class JsonConfigurationBase : IConfigurationBase
    {
        public JToken Node { get; protected set; }

        public JsonConfigurationBase(JToken node)
        {
            Node = node;
        }

        public string this[string key]
        {
            get
            {
                JsonConfigurationBase current = this;
                var parts = key.Split(new []{ '.' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var part in parts)
                {
                    current = (JsonConfigurationBase) current.GetSection(part);
                }

                GuardLoaded();
                return current.Node.Value<string>();
            }
            set
            {
                GuardLoaded();
                Node[key] = value;
            }
        }

        public IConfigurationSection GetSection(string key)
        {
            JsonConfigurationBase currentNode = this;
            var parts = key.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
                return new JsonConfigurationSection(Node[key]);

            foreach (var part in parts)
            {
                currentNode = (JsonConfigurationSection) currentNode.GetSection(part);
            }

            GuardLoaded();
            return (IConfigurationSection) currentNode;
        }

        public IConfigurationSection CreateSection(string key)
        {
            return GetSection(key);
        }

        public bool RemoveSection(string key)
        {
            Node[key].Remove();
            return true;
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            GuardLoaded();

            List<IConfigurationSection> sections = new List<IConfigurationSection>();
            foreach (JToken node in Node.Children()) sections.Add(new JsonConfigurationSection(node));

            return sections;
        }

        public void GuardLoaded()
        {
            if (Node == null) throw new ConfigurationNotLoadedException();
        }


        public virtual T Get<T>() => Node.ToObject<T>();
        public T Get<T>(T defaultValue)
        {
            if (!TryGet(out T val))
            {
                val = defaultValue;
            }

            return val;
        }

        public virtual void Set(object o)
        {
            if (!(Node is JProperty)) throw new Exception("Can not set value of: " + Node.Path);

            ((JProperty)Node).Value = new JValue(o);
        }

        public bool TryGet<T>(out T value)
        {
            value = default(T);
            try
            {
                value = Get<T>();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerator<IConfigurationSection> GetEnumerator()
        {
            return GetChildren().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}