using System;
using UnityEngine;

namespace Rocket.API.Providers
{
    public class RocketProviderProxyAttribute
    {
        internal RocketProviderProxyAttribute(Type provider)
        {

        }
    }

    public class RocketProviderTypeAttribute
    {

    }

    public class RocketProviderAttribute
    {
        internal RocketProviderAttribute(Type provider)
        {

        }
    }

    public interface IRocketProviderBase
    {
        void Unload();
        void Load(bool isReload = false);
    }
}
