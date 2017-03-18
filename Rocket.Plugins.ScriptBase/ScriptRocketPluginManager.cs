using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rocket.API.Collections;
using Rocket.API.Commands;
using Rocket.API.Logging;
using Rocket.API.Plugins;

namespace Rocket.Plugins.ScriptBase
{
    /// <summary>
    /// <p>The PluginManager for a scripting implementation</p>
    /// <p>To be implemented by the script implementation.</p>
    /// </summary>
    public class ScriptRocketPluginManager : IRocketPluginManager
    {
        private readonly ScriptEngine _engine;
        public static ScriptRocketPluginManager Instance { get; private set; }

        private readonly List<IRocketPlugin> _plugins = new List<IRocketPlugin>();

        public ScriptRocketPluginManager(ScriptEngine engine)
        {
            _engine = engine;
            Commands = new RocketCommandList(this);
            Instance = this;
        }

        public List<IRocketPlugin> GetPlugins() => _plugins;

        public IRocketPlugin GetPlugin(string name)
            => _plugins.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        public string PluginsDirectory => _engine.PluginsDir;
        public void Load(string pluginDirectory, string languageCode)
        {
            //todo: languageCode impl
            IScriptContext context = null;
            var res = _engine.LoadPluginFromDirectory(pluginDirectory, ref context);
            string plName = new DirectoryInfo(pluginDirectory).Name;

            switch (res.ExecutionResult)
            {
                case ScriptExecutionResult.SUCCESS:
                    _plugins.Add(context.Plugin);
                    break;
                default:
                    Logger.Error($"[${_engine.Name}PluginManager] Failed to load script plugin: {plName} ({res.ExecutionResult})", res.Exception);
                    break;
            }

            _plugins.Add(context.Plugin);
        }

        public void Reload()
        {
            //todo
            throw new System.NotImplementedException();
        }

        public void Unload()
        {
            //todo
            throw new System.NotImplementedException();
        }

        public string GetPluginDirectory(string name)
            => Path.Combine(PluginsDirectory, name);

        public RocketCommandList Commands { get; }
        public InitialiseDelegate Initialise { get; set; }
        public ScriptEngine ScriptEngine => _engine;
    }
}