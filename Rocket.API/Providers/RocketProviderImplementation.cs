using System;

namespace Rocket.API.Providers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RocketProviderImplementationAttribute : Attribute
    {
        public RocketProviderImplementationAttribute()
        {
            AutoLoad = false;
        }

        internal RocketProviderImplementationAttribute(bool autoLoad)
        {
            AutoLoad = autoLoad;
        }

        public bool AutoLoad { get; }
    }
}