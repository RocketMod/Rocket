using Rocket.API.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API.Assets;

namespace Rocket.API.Plugins
{
    public interface IRocketPluginManager
    {
        List<IRocketPlugin> GetPlugins();
        IRocketPlugin GetPlugin(string name);
        void Reload();
        string GetPluginDirectory(string name);
        RocketCommandList Commands { get; }
        string Name { get; }

        void LoadPlugin(IRocketPlugin rocketPlugin);

        IAsset<IRocketPluginConfiguration> GetPluginConfiguration(IRocketPlugin plugin,Type configuration,string name = "");
        IAsset<IRocketPluginConfiguration> GetPluginTranslation(IRocketPlugin plugin);
    }
}
