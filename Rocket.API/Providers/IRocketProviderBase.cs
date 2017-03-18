using UnityEngine;

namespace Rocket.API.Providers
{
    public interface IRocketProviderBase
    {
        void Unload();
        void Load();
    }
}
