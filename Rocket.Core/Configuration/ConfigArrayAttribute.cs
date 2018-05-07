using System;

namespace Rocket.Core.Configuration
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ConfigArrayAttribute : Attribute
    {
        public string ElementName { get; set; }
    }
}