using UnityEngine;

namespace Rocket.API.Providers
{
    public class RocketProviderBase : MonoBehaviour
    {
        public virtual bool Unload()
        {
            return false;
        }
        public virtual bool Load()
        {
            return false;
        }
    }
}
