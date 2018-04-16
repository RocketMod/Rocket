using System.IO;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rocket.Core.Configuration.JsonNetBase;

namespace Rocket.Core.Configuration.Xml
{
    public class XmlConfiguration : JsonNetConfigurationBase
    {
        private const string ConfigRoot = "Config";
        protected override void LoadFromFile(string file)
        {
            string xml  = File.ReadAllText(file);
            LoadFromXml(xml);
        }

        public void LoadFromXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            var json = JsonConvert.SerializeXmlNode(doc);

            Node = JObject.Parse(json, new JsonLoadSettings
            {
                CommentHandling = CommentHandling.Ignore,
                LineInfoHandling = LineInfoHandling.Ignore
            }).GetValue(ConfigRoot).DeepClone();

            IsLoaded = true;
        }

        protected override void SaveToFile(string file)
        {
            var clone = Node.DeepClone();
            var xml = new
            {
                @version = "1.0",
                @encoding = "UTF-8",
                @standalone = true
            };
            JObject o = new JObject
            {
                {"?xml", new JObject(xml)},
                {ConfigRoot, clone}
            };

            var json = o.ToString();

            XmlDocument doc = JsonConvert.DeserializeXmlNode(json);
            File.WriteAllText(file, doc.ToString());
        }
    }
}