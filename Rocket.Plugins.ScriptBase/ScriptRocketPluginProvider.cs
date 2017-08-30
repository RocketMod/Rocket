using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Rocket.API.Providers;
using Rocket.Core;
using Rocket.API.Plugins;
using Rocket.API.Logging;

namespace Rocket.Plugins.ScriptBase
{
    /// <summary>
    /// <p>The PluginProvider for a scripting implementation</p>
    /// <p>To be implemented by the script implementation.</p>
    /// </summary>
    public class ScriptRocketPluginProvider : IPluginProvider
    {
        private readonly ScriptEngine _engine;
        public static ScriptRocketPluginProvider Instance { get; private set; }

        private readonly List<IPlugin> _plugins = new List<IPlugin>();

        ILoggingProvider Logging;

        public ScriptRocketPluginProvider(ScriptEngine engine)
        {
            Logging = R.Providers.GetProvider<ILoggingProvider>();
            _engine = engine;
            Instance = this;
        }

        public List<IPlugin> GetPlugins() => _plugins;

        public IPlugin GetPlugin(string name)
            => _plugins.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        public string PluginsDirectory => _engine.PluginsDir;


        public void Unload(bool isReload = false)
        {
            //todo
            throw new System.NotImplementedException();
        }

        public string GetPluginDirectory(string name)
            => Path.Combine(PluginsDirectory, name);

        public ScriptEngine ScriptEngine => _engine;

        public void Load(bool isReload = false)
        {
            if (!Directory.Exists(PluginsDirectory))
                Directory.CreateDirectory(PluginsDirectory);
        }

        public ReadOnlyCollection<Type> Providers => new List<Type>().AsReadOnly();
        public ReadOnlyCollection<IPlugin> Plugins => _plugins.AsReadOnly();
        public ReadOnlyCollection<Type> LoadProviders()
        {
            return Providers;
        }

        public void LoadPlugins()
        {
            IScriptContext context = null;
            var res = _engine.LoadPluginFromDirectory(PluginsDirectory, ref context);
            string plName = new DirectoryInfo(PluginsDirectory).Name;

            switch (res.ExecutionResult)
            {
                case ScriptExecutionResult.SUCCESS:
                    _plugins.Add(context.Plugin);
                    break;
                default:
                    Logging.Log(LogLevel.ERROR, $"[${_engine.Name}PluginProvider] Failed to load script plugin: {plName} ({res.ExecutionResult})", res.Exception);
                    break;
            }

            _plugins.Add(context.Plugin);
        }
    }
}