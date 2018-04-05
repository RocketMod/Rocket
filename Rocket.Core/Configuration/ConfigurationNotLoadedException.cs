using System;

namespace Rocket.Core.Configuration
{
    public class ConfigurationNotLoadedException : Exception
    {
        public ConfigurationNotLoadedException() : base(
            "The configuration has not been loaded yet. Did you forget to call Reload()?")
        {

        }
    }
}