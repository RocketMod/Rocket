using Rocket.API.Commands;
using System.Collections.Generic;
using Rocket.API.Assets;
using Rocket.API.Collections;
using Rocket.API.Plugins;

namespace Rocket.API.Providers
{
    public interface IRocketPluginProvider
    {
        IRocketCommandProvider CommandProvider { get; }
        List<IRocketPlugin> GetPlugins();
        IRocketPlugin GetPlugin(string name);
        void Load();
        void Unload();
    }
}
