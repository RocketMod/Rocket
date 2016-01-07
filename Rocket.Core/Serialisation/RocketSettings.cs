using Rocket.Core.Assets;
using System.Xml.Serialization;
using System;

namespace Rocket.Core.Serialization
{
    public sealed class RemoteConsole
    {
        [XmlAttribute]
        public bool Enabled = false;
        [XmlAttribute]
        public short Port = 27115;
        [XmlAttribute]
        public string Password = "changeme";
    }

    public sealed class AutomaticShutdown
    {
        [XmlAttribute]
        public bool Enabled = false;
        [XmlAttribute]
        public int Interval = 86400;
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
        [XmlElement("RCON")]
        public RemoteConsole RCON;

        [XmlElement("AutomaticShutdown")]
        public AutomaticShutdown AutomaticShutdown;

        [XmlElement("WebConfigurations")]
        public WebConfigurations WebConfigurations;

        [XmlElement("WebPermissions")]
        public WebPermissions WebPermissions;

        [XmlElement("LanguageCode")]
        public string LanguageCode;

        [XmlElement("MaxFrames")]
        public int MaxFrames;
        
        public void LoadDefaults()
        {
            RCON = new RemoteConsole();
            AutomaticShutdown = new AutomaticShutdown();
            WebConfigurations = new WebConfigurations();
            WebPermissions = new WebPermissions();
            LanguageCode = "en";
            MaxFrames = 60;
        }
    }
}