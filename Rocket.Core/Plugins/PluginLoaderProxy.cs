using Rocket.API.DependencyInjection;
using Rocket.API.Plugins;
using Rocket.Core.ServiceProxies;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                /*
                if (! pluginManager.PluginExistsAsync(name).GetAwaiter().GetResult())
                    continue;
                */

                IPlugin plugin = pluginManager.GetPlugin(name);
                if (plugin != null)
                    return plugin;
            }

            return null;
        }

        public async Task<bool> PluginExistsAsync(string name)
        {
            foreach(var service in ProxiedServices)
                if (await service.PluginExistsAsync(name))
                    return true;

            return false;
        }


        public async Task InitAsync()
        {
            foreach (IPluginLoader pluginManager in ProxiedServices)
                await pluginManager.InitAsync();
        }

        public IEnumerable<IPlugin> Plugins => ProxiedServices.SelectMany(c => c.Plugins);

        public Task ExecuteSoftDependCodeAsync(string pluginName, Func<IPlugin, Task> action)
             => throw new NotSupportedException("Not supported on proxies");

        public Task<bool> ActivatePluginAsync(string name)
            => throw new NotSupportedException("Activate plugins is not supported through proxy");

        public Task<bool> DeactivatePluginAsync(string name)
            => throw new NotSupportedException("Unloading plugins is not supported through proxy");

        public string ServiceName => "ProxyPlugins";
    }
}