using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rocket.API.Commands;
using Rocket.API.Logging;
using Rocket.API.Plugins;
using Rocket.Plugins.ScriptBase;

namespace Rocket.Plugins.JavaScript
{
    public class JavascriptRocketPluginManager : ScriptRocketPluginManager
    {
        private readonly JavaScriptEngine _engine;
        public static JavascriptRocketPluginManager Instance { get; private set; }

        private readonly List<IRocketPlugin> _plugins = new List<IRocketPlugin>();

        public JavascriptRocketPluginManager(JavaScriptEngine engine)
        {
            _engine = engine;
            Commands = new RocketCommandList(this);
            Instance = this;
        }

        public override List<IRocketPlugin> GetPlugins() => _plugins;

        public override IRocketPlugin GetPlugin(string name)
            => _plugins.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        public override string PluginsDirectory => _engine.PluginsDir;
        public override void Load(string pluginDirectory, string languageCode)
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
                case ScriptExecutionResult.FILE_NOT_FOUND:
                case ScriptExecutionResult.LOAD_FAILED:
                case ScriptExecutionResult.FAILED_MISC:
                case ScriptExecutionResult.FAILED_EXCEPTION:
                    Logger.Error($"[${_engine.Name}Manager] Failed to load plugin: " + plName, res.Exception);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _plugins.Add(context.Plugin);
        }

        public override void Reload()
        {
            //todo
            throw new System.NotImplementedException();
        }

        public override void Unload()
        {
            //todo
            throw new System.NotImplementedException();
        }

        public override string GetPluginDirectory(string name)
            =>   Path.Combine(PluginsDirectory, name);

        public override RocketCommandList Commands { get; }
        public override InitialiseDelegate Initialise { get; set; }
        public override ScriptEngine ScriptEngine => _engine;
    }
}