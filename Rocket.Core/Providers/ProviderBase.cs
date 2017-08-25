using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Core.Providers
{
    public abstract class ProviderBase
    {
        protected abstract void OnLoad(ProviderManager providerManager);
        protected abstract void OnUnload();
        internal void Load(ProviderManager providerManager) { OnLoad(providerManager); }
        internal void Unload() { OnUnload(); }
    }
}
