using System;
using System.IO;
using Rocket.Plugins.ScriptBase;

namespace Rocket.Plugins.ClearScript
{
    public abstract class ClearScriptEngine : ScriptEngine
    {
        protected ClearScriptEngine()
        {
            PluginManager = new ScriptRocketPluginManager(this);
        }

        protected override ScriptResult ExecuteFile(string path, string entryPoint, ref IScriptContext context, ScriptPluginMeta meta,
            bool createPluginInstanceOnNull = false)
        {
            Microsoft.ClearScript.ScriptEngine engine = (Microsoft.ClearScript.ScriptEngine)context?.BindingsObj;
            if (context == null)
            {
                engine = CreateNewEngine();
                var pl = new ScriptRocketPlugin();
                context = new ScriptContext<Microsoft.ClearScript.ScriptEngine>(pl, this, engine);
                pl.ScriptContext = context;
                RegisterTypes(context);
            }

            if (engine == null)
            {
                API.Logging.Logger.Warn("ClearScript ScriptEngine equals null, script: " + path);
                return new ScriptResult(ScriptExecutionResult.FAILED_MISC);
            }
            
            var ret = engine.Invoke(entryPoint, ScriptInitHelper);
            var res = new ScriptResult(ScriptExecutionResult.SUCCESS);
            res.HasReturn = true; //unknown thanks to JavaScripts non existant type safety, yay!
            res.Return = ret;
            return res;
        }

        public override void RegisterType(string name, Type type, IScriptContext context)
        {
            var eng = (Microsoft.ClearScript.ScriptEngine)context.BindingsObj;
            eng.AddHostType(name, type);
        }

        protected abstract Microsoft.ClearScript.ScriptEngine CreateNewEngine();

        public override ScriptRocketPluginManager PluginManager { get; }
        public override ScriptPluginMeta GetPluginMeta(string path)
        {
            //todo get this from a meta file (e.g. PluginInfo.xml)
            return new ScriptPluginMeta
            {
                Author = string.Empty,
                Name = new DirectoryInfo(path).Name,
                EntryPoint = "Load"
            };
        }
    }
}