using System.IO;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Rocket.Core.Configuration.JsonNetBase;

namespace Rocket.Core.Configuration.Xml
{
    public class XmlConfiguration : JsonNetConfigurationBase
    {
        protected override void LoadFromFile(string file)
        {
            string xml  = File.ReadAllText(file);
            LoadFromXml(xml);
        }

        public void LoadFromXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            var json = JsonConvert.DeserializeXmlNode(xml).ToString();

            Node = JObject.Parse(json, new JsonLoadSettings
            {
                CommentHandling = CommentHandling.Ignore,
                LineInfoHandling = LineInfoHandling.Ignore
            });

            IsLoaded = true;
        }

        protected override void SaveToFile(string file)
        {
            var json = Node.ToString();

            XmlDocument doc = JsonConvert.DeserializeXmlNode(json);
            File.WriteAllText(file, doc.ToString());
        }
    }
}