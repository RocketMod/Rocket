using System;
using Rocket.Plugins.ScriptBase;

namespace Rocket.Plugins.ClearScript
{
    public abstract class ClearScriptEngine : ScriptEngine
    {
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

            engine.AllowReflection = false; //todo: make configurable
            engine.EnableAutoHostVariables = true;
            var ret = engine.Invoke(entryPoint, GetScriptIniter(context));
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
    }
}