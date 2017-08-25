using Rocket.API.Providers;
using Rocket.API.Providers.Plugins;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

namespace Rocket.Core.Providers.Plugin
{
    [ProviderProxy]
    public class RocketPluginProviderProxy : ProviderBase, IRocketPluginProvider
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

        public void LoadPlugins()
        {
            foreach (var prov in R.Providers.GetProviders<IRocketPluginProvider>())
            {
                prov.LoadPlugins();
            }
        }

        protected override void OnLoad(ProviderManager providerManager)
        {
            throw new NotImplementedException();
        }

        protected override void OnUnload()
        {
            throw new NotImplementedException();
        }
    }
}