using System;
using System.Linq;
using UnityEngine;

namespace Rocket.API.Providers
{
    public class RocketProviderProxyAttribute : Attribute
    {
        public Type Provider { get; private set; }
        internal RocketProviderProxyAttribute(Type provider = null)
        {
            if (provider == null) {
                Provider = GetType().GetInterfaces().FirstOrDefault();
            }
            else {
                Provider = provider;
            }
        }
    }

    public class RocketProviderAttribute : Attribute { }

    public interface IRocketProviderBase
    {
        void Unload();
        void Load(bool isReload = false);
    }
}
