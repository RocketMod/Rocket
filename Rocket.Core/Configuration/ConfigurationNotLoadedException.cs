using System;

namespace Rocket.Core.Configuration
{
    public class ConfigurationNotLoadedException : Exception
    {
        public ConfigurationNotLoadedException() : base(
            "The configuration or the section has not been loaded.") { }
    }
}