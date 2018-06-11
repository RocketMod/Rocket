using System;

namespace Rocket.Core.Configuration
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ConfigArrayAttribute : Attribute
    {
        public ConfigArrayAttribute()
        {
            
        }

        public ConfigArrayAttribute(string elementName)
        {
            ElementName = elementName;
        }

        public string ElementName { get; set; }
    }
}