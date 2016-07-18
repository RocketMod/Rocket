using Rocket.API;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Rocket.Core.Serialization
{
    public sealed class RocketCommands : IDefaultable
    {
        public void LoadDefaults()
        {
            CommandMappings = new List<CommandMapping>();
        }

        [XmlArray("Commands")]
        [XmlArrayItem("Command")]
        public List<CommandMapping> CommandMappings = new List<CommandMapping>();
    }
}