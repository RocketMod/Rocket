using System;

namespace Rocket.API.Providers
{
    public class RocketProviderProxyAttribute : Attribute
    {
        public Type Provider { get; set; }
        internal RocketProviderProxyAttribute(Type provider = null)
        {
            if (provider != null)
            {
                Provider = provider;
            }
        }
    }

    public interface IRocketProviderBase
    {
        void Unload(bool isReload = false);
        void Load(bool isReload = false);
    }
}
