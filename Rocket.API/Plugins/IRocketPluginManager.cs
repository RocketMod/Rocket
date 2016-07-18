using Rocket.API.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.API.Plugins
{
    public delegate void PluginsLoaded();
    public interface IRocketPluginManager<T> where T : IRocketPlugin
    {
        event PluginsLoaded OnPluginsLoaded;
        List<T> GetPlugins();
        T GetPlugin(string name);
        void Reload();
        string GetPluginDirectory(string name);
        List<IRocketCommand> GetCommands(T plugin);
    }
}
