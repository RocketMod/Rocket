using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.API.Providers
{
    public abstract class ProviderBase
    {
        protected virtual void OnLoad(IProviderManager providerManager)
        {
            
        }

        protected virtual void OnUnload()
        {
            
        }
        internal void Load(IProviderManager providerManager) { OnLoad(providerManager); }
        internal void Unload() { OnUnload(); }
    }
}
