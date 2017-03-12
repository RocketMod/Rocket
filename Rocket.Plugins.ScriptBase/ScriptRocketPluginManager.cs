using System.Collections.Generic;
using Rocket.API.Commands;
using Rocket.API.Plugins;

namespace Rocket.Plugins.ScriptBase
{
    /// <summary>
    /// <p>The PluginManager for a scripting implementation</p>
    /// <p>To be implemented by the script implementation.</p>
    /// </summary>
    public abstract class ScriptRocketPluginManager : IRocketPluginManager
    {
        public abstract List<IRocketPlugin> GetPlugins();

        public abstract IRocketPlugin GetPlugin(string name);

        public abstract string PluginsDirectory { get; }

        public abstract void Load(string pluginDirectory, string languageCode);

        public abstract void Reload();

        public abstract void Unload();

        public abstract string GetPluginDirectory(string name);

        public abstract RocketCommandList Commands { get; }
        public abstract InitialiseDelegate Initialise { get; set; }
    }
}