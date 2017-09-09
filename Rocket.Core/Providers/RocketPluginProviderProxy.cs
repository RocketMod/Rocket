using Rocket.API.Providers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using Rocket.API.Plugins;

namespace Rocket.Core.Providers.Plugin
{
    public class RocketPluginProviderProxy : ProxyBase<IPluginProvider>, IPluginProvider
    {
        public ReadOnlyCollection<IPlugin> Plugins
        {
            get
            {
                var list = new List<IPlugin>();
                foreach (var prov in R.Providers.GetProviders<IPluginProvider>())
                {
                    list.AddRange(prov.Plugins);
                }

                return list.AsReadOnly();
            }
        }

        public IPlugin GetPlugin(string name)
        {
            foreach (var prov in R.Providers.GetProviders<IPluginProvider>())
            {
                var pl = prov.GetPlugin(name);
                if (pl != null)
                    return pl;
            }

            return null;
        }

        public void LoadPlugins()
        {
            foreach (var prov in R.Providers.GetProviders<IPluginProvider>())
            {
                prov.LoadPlugins();
            }
        }
    }
}