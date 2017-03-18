using System.Collections.Generic;
using Rocket.API.Plugins;

namespace Rocket.API.Providers
{
    public interface IRocketPluginProvider
    {
        IRocketCommandProvider CommandProvider { get; }
        List<IRocketPlugin> GetPlugins();
        IRocketPlugin GetPlugin(string name);
        string GetPluginDirectory(string name);
        bool Load();
        bool Unload();
    }
}
