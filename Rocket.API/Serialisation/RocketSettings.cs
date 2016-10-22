using Rocket.API;
using System.Xml.Serialization;

namespace Rocket.Core.Serialization
{
    public sealed class RemoteConsole
    {
        [XmlAttribute]
        public bool Enabled = false;

        [XmlAttribute]
        public ushort Port = 27115;

        [XmlAttribute]
        public string Password = "changeme";

        [XmlAttribute]
        public string Username = "username";
    }

    public sealed class WebPermissions
    {
        [XmlAttribute]
        public bool Enabled = false;

        [XmlAttribute]
        public string Url = "";

        [XmlAttribute]
        public int Interval = 180;
    }

    public sealed class WebConfigurations
    {
        [XmlAttribute]
        public bool Enabled = false;

        [XmlAttribute]
        public string Url = "";
    }

    public sealed class RocketSettings : IDefaultable
    {
        [XmlElement("RPC")]
        public RemoteConsole RPC = new RemoteConsole();

        [XmlElement("WebConfigurations")]
        public WebConfigurations WebConfigurations = new WebConfigurations();

        [XmlElement("WebPermissions")]
        public WebPermissions WebPermissions = new WebPermissions();

        [XmlElement("LanguageCode")]
        public string LanguageCode = "en";

        [XmlElement("MaxFrames")]
        public int MaxFrames = 60;

        public void LoadDefaults()
        {
            RPC = new RemoteConsole();
            WebConfigurations = new WebConfigurations();
            WebPermissions = new WebPermissions();
            LanguageCode = "en";
            MaxFrames = 60;
        }
    }
}