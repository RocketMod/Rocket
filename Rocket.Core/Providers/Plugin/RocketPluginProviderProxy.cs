using Rocket.API.Providers;
using Rocket.API.Providers.Plugins;
using System;
using System.Collections.ObjectModel;

namespace Rocket.Core.Providers.Plugin
{
    [RocketProviderProxy]
    public class RocketPluginProviderProxy : IRocketPluginProvider
    {
        public ReadOnlyCollection<IRocketPlugin> Plugins
        {
            get { return null; }
        }

        public ReadOnlyCollection<Type> Providers {
            get { return null; }
        }

        public IRocketPlugin GetPlugin(string name)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<IRocketPlugin> GetPlugins()
        {
            throw new NotImplementedException();
        }

        public void Load(bool isReload = false)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<Type> LoadProviders()
        {
            throw new NotImplementedException();
        }

        public void Unload(bool isReloading)
        {
            throw new NotImplementedException();
        }
    }
}