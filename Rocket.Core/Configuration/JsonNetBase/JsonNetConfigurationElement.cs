using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Rocket.API.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Serialization;

namespace Rocket.Core.Configuration.JsonNetBase
{
    public abstract class JsonNetConfigurationElement : IConfigurationElement
    {
        private static readonly JsonSerializer expandoSupportedSerializer;

        static JsonNetConfigurationElement()
        {
            var expConverter = new ExpandoObjectConverter();
            expandoSupportedSerializer = JsonSerializer.CreateDefault();
            expandoSupportedSerializer.Converters.Add(expConverter);
        }

        protected JsonNetConfigurationElement(IConfiguration root)
        {
            Root = root;
        }

        protected JsonNetConfigurationElement(IConfiguration root, IConfigurationElement parentElement, JToken node, SectionType type) : this(root)
        {
            ParentElement = parentElement;
            Node = node ?? throw new ArgumentNullException(nameof(node));
            Type = type;
        }

        public JToken Node { get; protected set; }
        public SectionType Type { get; }

        public IConfigurationSection this[string path] => GetSection(path);
        public IConfigurationElement ParentElement { get; }
        public IConfiguration Root { get; protected set; }

        public IConfigurationSection GetSection(string path)
        {
            if (path.Trim().Equals("") && this is IConfigurationSection section)
                return section;

            GuardLoaded();
            GuardPath(path);

            JsonNetConfigurationElement currentNode = this;
            string[] parts = path.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 1)
            {
                string key = parts[0];

                //Path == "" on Root
                string parentPath = string.IsNullOrEmpty(Path) ? "" : Path + ".";

                if (Node is JObject o && !o.ContainsKey(key))
                    throw new KeyNotFoundException($"Path \"{parentPath}{path}\" doesn\'t exist!");

                JToken childNode = Node[key];
                if (childNode is JValue)
                    childNode = childNode.Parent;

                SectionType sectionType;
                if (childNode is JArray)
                    sectionType = SectionType.Array;
                else if (childNode is JObject)
                    sectionType = SectionType.Object;
                else
                    sectionType = SectionType.Value;

                return new JsonNetConfigurationSection(Root, this, childNode, key, sectionType);
            }

            foreach (string part in parts)
                currentNode = (JsonNetConfigurationSection)currentNode.GetSection(part);

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

                if (i == parts.Length - 1)
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
                else
                    o.Add(part, new JObject());

                current = current[part];
                i++;
            }

            return GetSection(path);
        }

        public bool DeleteSection(string path)
        {
            GuardLoaded();
            GuardPath(path);

            JToken node = ((JsonNetConfigurationElement)GetSection(path)).Node;
            JObject parent = (JObject)((JsonNetConfigurationElement)GetSection(path).ParentElement).Node;
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
                if (childPath.StartsWith("@"))
                    continue;

                sections.Add(GetSection(childPath));
            }

            return sections;
        }

        public abstract string Path { get; }

        public virtual T Get<T>()
        {
            return (T)Get(typeof(T));
        }

        public dynamic Get()
        {

            return Node.ToObject<ExpandoObject>(expandoSupportedSerializer);
        }

        public object Get(Type targetType)
        {
            var target = FormatterServices.GetUninitializedObject(targetType);

            JsonSerializer serializer = new JsonSerializer();
            serializer.Populate(Node.CreateReader(), target);
            return target;
        }

        public T Get<T>(T defaultValue)
        {
            GuardLoaded();

            if (!TryGet(out T val))
                val = defaultValue;

            return val;
        }

        public object Get(Type t, object defaultValue)
        {
            GuardLoaded();

            if (!TryGet(t, out object val))
            {
                val = defaultValue;
            }

            return val;
        }

        public virtual void Set(object value)
        {
            JToken node = Node;

            if (node is JArray array)
            {
                foreach (JToken child in array.ToList() /* ToList() to make a simple clone */)
                {
                    child.Remove();
                }

                if (value is IEnumerable enumerable)
                {
                    foreach (object child in enumerable)
                        try
                        {
                            array.Add(child is JToken ? child : new JValue(child));
                        }
                        catch (Exception)
                        {
                            array.Add(JObject.FromObject(child));
                        }

                    return;
                }

                node = node.Parent;
            }

            if (node is JObject @object)
            {
                JObject obj = JObject.FromObject(value);
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

        public abstract IConfigurationElement Clone();

        public IEnumerator<IConfigurationSection> GetEnumerator() => GetChildren().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void GuardLoaded()
        {
            if (Node == null)
            {
                throw new ConfigurationNotLoadedException(Root);
            }
        }

        public void GuardPath(string path)
        {
            string[] parts = path.Split('.');
            if (parts.Any(c => long.TryParse(c, out var _)))
            {
                throw new Exception("Paths can not contain sections which are numbers. Path: " + path);
            }

            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Configuration paths can not be null or empty");
            }
        }

        public static void DeepCopy(JObject fromParent, JObject toParent, bool overrideOnTypeMismatch = true)
        {
            if (fromParent.Type != toParent.Type)
                return;

            foreach (JToken t in fromParent.Children())
            {
                start:
                JToken fromChild = t;
                string path = fromChild.Path.Replace(fromParent.Path + ".", "");

                if (!toParent.ContainsKey(path))
                {
                    toParent.Add(fromChild);
                    continue;
                }

                JToken toChild = toParent[path].Parent ?? toParent[path];

                if (fromChild is JValue || fromChild is JArray)
                    fromChild = toChild.Parent;

                if (toChild is JValue || toChild is JArray)
                    toChild = toChild.Parent;

                if (fromChild is JObject fromObj && toChild is JObject toObj)
                {
                    DeepCopy(fromObj, toObj);
                    continue;
                }

                if (fromChild is JProperty fromProperty && toChild is JProperty toProperty)
                {
                    toProperty.Value = fromProperty.Value;
                    continue;
                }

                if (fromChild.GetType() != toChild.GetType())
                {
                    if (overrideOnTypeMismatch)
                    {
                        toParent.Remove(path);
                        goto start;
                    }

                    continue;
                }

                throw new Exception("Copy not copy: from \"" + fromChild.Type + "\" to \"" + toChild.Type + "\"");
            }
        }
    }
}