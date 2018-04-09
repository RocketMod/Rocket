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

        public IConfigurationSection this[string path] => GetSection(path);

        public IConfigurationSection GetSection(string path)
        {
            GuardLoaded();
            GuardPath(path);

            JsonConfigurationBase currentNode = this;
            string[] parts = path.Split(new[] {'.'}, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1) return new JsonConfigurationSection(Node[parts[0]], parts[0]);

            foreach (string part in parts) currentNode = (JsonConfigurationSection) currentNode.GetSection(part);

            return (IConfigurationSection) currentNode;
        }

        public IConfigurationSection CreateSection(string path)
        {
            GuardPath(path);
            return GetSection(path);
        }

        public bool RemoveSection(string path)
        {
            GuardPath(path);
            Node[path].Remove();
            return true;
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            GuardLoaded();

            List<IConfigurationSection> sections = new List<IConfigurationSection>();
            foreach (JToken node in Node.Children())
            {
                string childPath = node.Path.Replace(Node.Path + ".", "");
                sections.Add(GetSection(childPath));
            }

            return sections;
        }

        public void GuardLoaded()
        {
            if (Node == null)
                throw new ConfigurationNotLoadedException();
        }

        public void GuardPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("Configuration paths can not be null or empty");
        }

        public virtual T Get<T>() => Node.ToObject<T>();
        public object Get(Type t) => Node.ToObject(t);

        public T Get<T>(T defaultValue)
        {
            if (!TryGet(out T val)) val = defaultValue;

            return val;
        }

        public object Get(Type t, object defaultValue)
        {
            if (!TryGet(t, out object val)) val = defaultValue;

            return val;
        }

        public virtual void Set(object o)
        {
            if (!(Node is JProperty)) throw new Exception("Can not set value of: " + Node.Path);

            ((JProperty) Node).Value = new JValue(o);
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

        public bool TryGet(Type t, out object value)
        {
            value = null;
            try
            {
                value = Get(t);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerator<IConfigurationSection> GetEnumerator() => GetChildren().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}