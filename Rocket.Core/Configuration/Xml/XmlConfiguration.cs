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

        protected override string FileEnding => "xml";

        protected override void LoadFromFile(string file)
        {
            string xml = System.IO.File.ReadAllText(file);
            LoadFromXml(xml);
        }

        public void LoadFromXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            string json = JsonConvert.SerializeXmlNode(doc);

            Node = JObject.Parse(json, new JsonLoadSettings
                          {
                              CommentHandling = CommentHandling.Ignore,
                              LineInfoHandling = LineInfoHandling.Ignore
                          })
                          .GetValue(ConfigRoot)
                          .DeepClone();

            IsLoaded = true;
        }

        protected override void SaveToFile(string file)
        {
            JToken clone = Node.DeepClone();
            var xml = new
            {
                version = "1.0",
                encoding = "UTF-8",
                standalone = true
            };
            JObject o = new JObject
            {
                {"?xml", new JObject(xml)},
                {ConfigRoot, clone}
            };

            string json = o.ToString();

            XmlDocument doc = JsonConvert.DeserializeXmlNode(json);
            System.IO.File.WriteAllText(file, doc.ToString());
        }
    }
}