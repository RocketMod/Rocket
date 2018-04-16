using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rocket.API.DependencyInjection;
using Rocket.API.Plugin;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Plugins
{
    public class ProxyPluginManager : ServiceProxy<IPluginManager>, IPluginManager
    {
        public ProxyPluginManager(IDependencyContainer container) : base(container) { }
        public IEnumerator<IPlugin> GetEnumerator()
        {
            return Plugins.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IPlugin GetPlugin(string name)
        {
            foreach (var pluginManager in ProxiedProviders)
            {
                if(!pluginManager.PluginExists(name))
                    continue;

                var plugin = pluginManager.GetPlugin(name);
                if (plugin != null)
                    return plugin;
            }

            return null;
        }

        public bool PluginExists(string name)
        {
            return ProxiedProviders.Any(c => c.PluginExists(name));
        }

        public void Init()
        {
            foreach(var pluginManager in ProxiedProviders)
                pluginManager.Init();
        }

        public bool LoadPlugin(string name)
        {
            throw new NotSupportedException("Load plugins is not supported through proxy");
        }

        public bool UnloadPlugin(string name)
        {
            throw new NotSupportedException("Unloading plugins is not supported through proxy");
        }

        public IEnumerable<IPlugin> Plugins => ProxiedProviders.SelectMany(c => c.Plugins);
        public bool ExecutePluginDependendCode(string pluginName, Action<IPlugin> action)
        {
            throw new NotSupportedException("Not supported on proxies");
        }
    }
}