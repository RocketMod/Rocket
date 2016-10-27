using Rocket.API.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API.Assets;

namespace Rocket.API.Plugins
{
    public delegate void InitialiseDelegate(string pluginDirectory, string librariesDirectory);
    public interface IRocketPluginManager
    {
        List<IRocketPlugin> GetPlugins();
        IRocketPlugin GetPlugin(string name);
        void Reload();
        string GetPluginDirectory(string name);
        RocketCommandList Commands { get; }
        void LoadPlugin(IRocketPlugin rocketPlugin);
        InitialiseDelegate Initialise { set; }
        IAsset<IRocketPluginConfiguration> GetPluginConfiguration(IRocketPlugin plugin,Type configuration,string name = "");
        IAsset<IRocketPluginConfiguration> GetPluginTranslation(IRocketPlugin plugin);
    }
}
