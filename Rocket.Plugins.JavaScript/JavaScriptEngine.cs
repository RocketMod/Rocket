using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.ClearScript.V8;
using Rocket.Core;
using Rocket.Plugins.ScriptBase;

namespace Rocket.Plugins.JavaScript
{
    public class JavaScriptEngine : ScriptEngine
    {
        public override string Name => "JavaScript";
        public override List<string> FileTypes => new List<string> { "js", "javascript" };
        public override string ShortName => "js";

        public JavaScriptEngine()
        {
            PluginManager = new JavascriptRocketPluginManager(this);
        }

        protected override ScriptResult ExecuteFile(string path, string entryPoint, ref IScriptContext context, ScriptPluginMeta meta,
            bool createPluginInstanceOnNull = false)
        {
            Microsoft.ClearScript.ScriptEngine engine = (Microsoft.ClearScript.ScriptEngine) context?.BindingsObj;
            if (context == null)
            {
                engine = new V8ScriptEngine();
                var pl = new JavaScriptRocketPlugin();
                context = new ScriptContext<Microsoft.ClearScript.ScriptEngine>(pl, this, engine);
                pl.ScriptContext = context;
            }

            if (engine == null)
            {
                API.Logging.Logger.Warn("ClearScript ScriptEngine equals null, script: " + path);
                return new ScriptResult(ScriptExecutionResult.FAILED_MISC);
            }

            engine.AddHostType("R", typeof(R));

            //todo bad code:
            var uType = Type.GetType("Rocket.Unturned.U");
            if (uType != null)
            {
                engine.AddHostType("U", uType);
            }

            var ret = engine.Invoke(entryPoint, ScriptInitHelper);
            var res = new ScriptResult(ScriptExecutionResult.SUCCESS);
            res.HasReturn = true; //unknown thanks to JavaScripts non existant type safety, yay!
            res.Return = ret;
            return res;
        }

        public override ScriptRocketPluginManager PluginManager { get; }
        protected override ScriptPluginMeta GetPluginMeta(string path)
        {
            //todo get this from a meta file (e.g. PluginInfo.xml)
            return new ScriptPluginMeta {
                Author = string.Empty,
                Name = new DirectoryInfo(path).Name,
                EntryPoint = "Load"
            };
        }
    }

    public class JavaScriptRocketPlugin : ScriptRocketPlugin
    {
        public override IScriptContext ScriptContext { get; set; }
    }
}