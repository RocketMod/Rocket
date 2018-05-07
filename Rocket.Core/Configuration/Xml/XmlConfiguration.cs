using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rocket.API.Configuration;
using Rocket.Core.Configuration.JsonNetBase;

namespace Rocket.Core.Configuration.Xml
{
    public class XmlConfiguration : JsonNetConfigurationBase
    {
        public string ConfigurationRoot { get; set; } = "Config";

        protected override string FileEnding => "xml";
        public override string Name => "Xml";

        protected override void LoadFromFile(string file)
        {
            string xml = File.ReadAllText(file);
            LoadFromXml(xml);
        }


        private void ApplyScheme(XmlElement element, Type scheme, XmlAttribute docAttribute)
        {
            foreach (var member in scheme.GetMembers(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!(member is PropertyInfo) && !(member is FieldInfo))
                    continue;

                PropertyInfo pi = member as PropertyInfo;
                FieldInfo fi = member as FieldInfo;

                var subElement = element
                                 .ChildNodes
                                 .Cast<XmlNode>()
                                 .FirstOrDefault(c => c is XmlElement subElem && subElem.Name.Equals(member.Name));

                if (subElement != null)
                {
                    if (member.GetCustomAttributes(typeof(ConfigArrayAttribute), true).Any())
                    {
                        subElement.Attributes?.Append(docAttribute);
                    }

                    var type = pi?.PropertyType ?? fi.FieldType;
                    ApplyScheme((XmlElement)subElement, type, docAttribute);
                }
            }
        }

        public void LoadFromXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            var docAttribute = doc.CreateAttribute("json", "Array", "http://james.newtonking.com/projects/json");
            docAttribute.InnerText = "true";

            if(Scheme != null)
                ApplyScheme(doc.DocumentElement, Scheme, docAttribute);
            
            string json = JsonConvert.SerializeXmlNode(doc);

            JToken tmp = JObject.Parse(json, new JsonLoadSettings
            {
                CommentHandling = CommentHandling.Ignore,
                LineInfoHandling = LineInfoHandling.Ignore
            });

            if (!string.IsNullOrEmpty(ConfigurationRoot))
                tmp = ((JObject)tmp).GetValue(ConfigurationRoot);
            else
                tmp = tmp.Children().Last().First();

            Node = tmp.DeepClone();
            IsLoaded = true;
        }

        protected override void SaveToFile(string file)
        {
            File.WriteAllText(file, ToXml());
        }

        public override IConfigurationElement Clone()
        {
            var config = new XmlConfiguration();
            config.LoadFromXml(ToXml());
            return config;
        }

        public string ToXml()
        {
            JToken clone = Node.DeepClone();
            string json = clone.ToString();

            //Appends <Config></Config>
            XmlDocument parent = new XmlDocument();
            XmlDeclaration xmlDeclaration = parent.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = parent.DocumentElement;
            parent.InsertBefore(xmlDeclaration, root);

            XmlElement configElement = root;

            if (!string.IsNullOrEmpty(ConfigurationRoot))
            {
                configElement = parent.CreateElement(string.Empty, ConfigurationRoot, string.Empty);
                parent.AppendChild(configElement);
            }

            XmlDocument jsonDocument = JsonConvert.DeserializeXmlNode(json);
            foreach (XmlNode elem in jsonDocument)
            {
                var copied = parent.ImportNode(elem, true);
                configElement.AppendChild(copied);
            }

            //Convert XmlDocument to String with a format
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "  ",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };
            using (XmlWriter writer = XmlWriter.Create(sb, settings))
            {
                parent.Save(writer);
            }
            return sb.ToString();
        }
    }

}