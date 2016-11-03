using Rocket.API;
using System.Xml.Serialization;

namespace Rocket.Core
{
    public sealed class RemoteConsole
    {
        [XmlAttribute]
        public bool Enabled = false;

        [XmlAttribute]
        public ushort Port = 27115;

        [XmlAttribute]
        public string Password = "changeme";
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

    public sealed class RocketSettings : IDefaultable
    {
        [XmlElement("RPC")]
        public RemoteConsole RPC = new RemoteConsole();

        [XmlElement("RCON")]
        public RemoteConsole RCON = new RemoteConsole();

        [XmlElement("WebPermissions")]
        public WebPermissions WebPermissions = new WebPermissions();

        [XmlElement("LanguageCode")]
        public string LanguageCode = "en";

        [XmlElement("MaxFrames")]
        public int MaxFrames = 60;

        public void LoadDefaults()
        {
            RPC = new RemoteConsole();
            RCON = new RemoteConsole();
            WebPermissions = new WebPermissions();
            LanguageCode = "en";
            MaxFrames = 60;
        }
    }
}