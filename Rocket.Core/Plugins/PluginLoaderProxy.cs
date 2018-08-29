using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.DependencyInjection;
using Rocket.API.Plugins;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Plugins
{
    public class PluginLoaderProxy : ServiceProxy<IPluginLoader>, IPluginLoader
    {
        public PluginLoaderProxy(IDependencyContainer container) : base(container) { }

        public IEnumerator<IPlugin> GetEnumerator() => Plugins.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public IPlugin GetPlugin(string name)
        {
            foreach (IPluginLoader pluginManager in ProxiedServices)
            {
                if (!pluginManager.PluginExists(name))
                    continue;

                IPlugin plugin = pluginManager.GetPlugin(name);
                if (plugin != null)
                    return plugin;
            }

            return null;
        }

        public bool PluginExists(string name)
        {
            return ProxiedServices.Any(c => c.PluginExists(name));
        }
        

        public void Init()
        {
            foreach (IPluginLoader pluginManager in ProxiedServices)
                pluginManager.Init();
        }

        public IEnumerable<IPlugin> Plugins => ProxiedServices.SelectMany(c => c.Plugins);

        public void ExecuteSoftDependCode(string pluginName, Action<IPlugin> action)
            => throw new NotSupportedException("Not supported on proxies");

        public bool ActivatePlugin(string name)
            => throw new NotSupportedException("Activate plugins is not supported through proxy");

        public bool DeactivatePlugin(string name)
            => throw new NotSupportedException("Unloading plugins is not supported through proxy");

        public string ServiceName => "ProxyPlugins";
    }
}