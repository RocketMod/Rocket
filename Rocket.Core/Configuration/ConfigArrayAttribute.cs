using System;

namespace Rocket.Core.Configuration
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class ConfigArrayAttribute : Attribute
    {
        /// <param name="elementName">The name of the element.</param>
        public ConfigArrayAttribute(string elementName)
        {
            ElementName = elementName;
        }

        public string ElementName { get; /*set;*/ }
    }
}