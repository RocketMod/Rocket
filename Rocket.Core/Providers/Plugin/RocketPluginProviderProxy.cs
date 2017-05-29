using Rocket.API.Providers;
using Rocket.API.Providers.Plugins;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rocket.Core.Providers.Plugin
{
    [RocketProviderProxy]
    public class RocketPluginProviderProxy : IRocketPluginProvider
    {
        public ReadOnlyCollection<IRocketPlugin> Plugins
        {
            get
            {
                var list = new List<IRocketPlugin>();
                foreach (var prov in R.Providers.GetProviders<IRocketPluginProvider>())
                {
                    list.AddRange(prov.Plugins);
                }

                return list.AsReadOnly();
            }
        }

        public IRocketPlugin GetPlugin(string name)
        {
            foreach (var prov in R.Providers.GetProviders<IRocketPluginProvider>())
            {
                var pl = prov.GetPlugin(name);
                if (pl != null)
                    return pl;
            }

            return null;
        }

        public void Load(bool isReload = false)
        {

        }

        public void Unload(bool isReloading)
        {
       
        }
    }
}