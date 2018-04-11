using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using Newtonsoft.Json.Linq;
using Rocket.API.Configuration;

namespace Rocket.Core.Configuration.Json
{
    public abstract class JsonConfigurationBase : IConfigurationBase
    {
        public JToken Node { get; protected set; }

        protected JsonConfigurationBase(IConfiguration root, IConfigurationBase parent, JToken node)
        {
            Root = root;
            Parent = parent;
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        protected JsonConfigurationBase(IConfiguration root)
        {
            Root = root;
        }

        public IConfigurationSection this[string path] => GetSection(path);
        public IConfigurationBase Parent { get; }
        public IConfiguration Root { get; protected set; }

        public IConfigurationSection GetSection(string path)
        {
            GuardLoaded();
            GuardPath(path);

            JsonConfigurationBase currentNode = this;
            string[] parts = path.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                string key = parts[0];

                //Path == "" on Root
                var parentPath = string.IsNullOrEmpty(Path) ? "" : Path + ".";

                if (Node is JObject o && !o.ContainsKey(key))
                    throw new KeyNotFoundException($"Path \"{parentPath}{path}\" doesn\'t exist!");

                var childNode = Node[key];
                if (childNode is JValue)
                    childNode = childNode.Parent;

                return new JsonConfigurationSection(Root, this, childNode, key);
            }

            foreach (string part in parts)
            {
                currentNode = (JsonConfigurationSection)currentNode.GetSection(part);
            }

            return (IConfigurationSection)currentNode;
        }

        public IConfigurationSection CreateSection(string path, SectionType type)
        {
            GuardLoaded();
            GuardPath(path);

            JToken current = Node;

            string[] parts = path.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            int i = 0;
            foreach (string part in parts)
            {
                if (!(current is JObject o) || o.ContainsKey(part))
                {
                    current = current[part];
                    i++;
                    continue;
                }

                if (i == (parts.Length - 1))
                {
                    switch (type)
                    {
                        case SectionType.Value:
                            o.Add(new JProperty(part, (string)null));
                            break;
                        case SectionType.Array:
                            o.Add(part, new JArray());
                            break;
                        case SectionType.Object:
                            o.Add(part, new JObject());
                            break;
                    }
                }
                else
                {
                    o.Add(part, new JObject());
                }

                current = current[part];
                i++;
            }

            return GetSection(path);
        }

        public bool RemoveSection(string path)
        {
            GuardLoaded();
            GuardPath(path);

            var node = ((JsonConfigurationBase)GetSection(path)).Node;
            var parent = (JObject)((JsonConfigurationBase)GetSection(path).Parent).Node;
            parent.Remove(node.Path.Replace(parent.Path + ".", ""));
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

        public abstract string Path { get; }

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
            GuardLoaded();

            if (!TryGet(out T val)) val = defaultValue;

            return val;
        }

        public object Get(Type t, object defaultValue)
        {
            GuardLoaded();

            if (!TryGet(t, out object val)) val = defaultValue;

            return val;
        }

        public virtual void Set(object value)
        {
            var node = Node;

            if (node is JArray array)
            {
                foreach (var child in array.ToList() /* ToList() to make a simple clone */)
                {
                    child.Remove();
                }

                if (value is IEnumerable enumerable)
                {
                    foreach (var child in enumerable)
                    {
                        array.Add(new JValue(child));
                    }
                    return;
                }

                node = node.Parent;
            }

            if (node is JObject @object)
            {
                var obj = JObject.FromObject(value);
                DeepCopy(obj, @object);
                return;
            }

            if (node is JProperty property)
            {
                property.Value = new JValue(value);
                return;
            }

            throw new NotSupportedException("Can not set values of non-properties");
        }

        public static void DeepCopy(JToken from, JToken to)
        {
            if (from.Type != to.Type)
                return;

            foreach (var child in from)
            {
                string path = child.Path.Replace(from.Path + ".", "");
                if (to is JObject o)
                {
                    if (!o.ContainsKey(path))
                        o.Add(child);
                    else
                        o[path] = child;
                }

                DeepCopy(child, to[path]);
            }
        }

        public bool TryGet<T>(out T value)
        {
            GuardLoaded();

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
            GuardLoaded();

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

        public bool ChildExists(string path)
        {
            try
            {
                GetSection(path);
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