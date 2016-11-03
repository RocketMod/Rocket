using Rocket.API.Commands;
using System.Collections.Generic;
using Rocket.API.Assets;
using Rocket.API.Collections;

namespace Rocket.API.Plugins
{
    public delegate void InitialiseDelegate(string pluginDirectory, string librariesDirectory);
    public interface IRocketPluginManager
    {
        List<IRocketPlugin> GetPlugins();
        IRocketPlugin GetPlugin(string name);
        string PluginsDirectory { get; }
        void Load(string pluginDirectory, string languageCode);
        void Reload();
        void Unload();
        string GetPluginDirectory(string name);
        RocketCommandList Commands { get; }
        InitialiseDelegate Initialise { set; }
    }
}
