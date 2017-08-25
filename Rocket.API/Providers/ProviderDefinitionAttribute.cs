using System;

namespace Rocket.API.Providers
{
    [AttributeUsage(AttributeTargets.Interface)]
    public class ProviderDefinitionAttribute : Attribute
    {
        public bool MultiInstance { get; set; } = true;
    }
}