using Rocket.API;
using System.Xml.Serialization;

namespace Rocket.Core
{
    public sealed class RocketSettings : IDefaultable
    {
        [XmlElement("MaxFrames")]
        public int MaxFrames = 60;

        public void LoadDefaults()
        {
            MaxFrames = 60;
        }
    }
}