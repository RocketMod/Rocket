using System;
using System.Collections.Generic;
using NLua;
using Rocket.Plugins.ScriptBase;

namespace Rocket.Plugins.NLua
{
    public class LuaScriptEngine : ScriptEngine
    {
        public override string Name => "Lua";
        public override List<string> FileTypes => new List<string> {"lua"};
        public override string ShortName => "Lua";

        protected override ScriptResult ExecuteFile(string path, string entryPoint, ref IScriptContext context, ScriptPluginMeta meta,
            bool createPluginInstanceOnNull = false)
        {
            LuaContext luaContext= (LuaContext) context?.BindingsObj;
            if (context == null)
            {
                var state = new Lua();
                state.LoadCLRPackage();
                var ctx = new LuaContext();
                ctx.State = state;

                ScriptRocketPlugin pl = null;
                if (createPluginInstanceOnNull)
                {
                    pl = new ScriptRocketPlugin();
                    pl.PluginMeta = meta;
                }

                context = new ScriptContext<LuaContext>(pl, this, ctx);

                if(pl != null)
                    pl.ScriptContext = context;

                RegisterContext(context);
            }

            if (luaContext?.State == null)
            {
                API.Logging.Logger.Warn("Lua state equals null, script: " + path);
                return new ScriptResult(ScriptExecutionResult.FAILED_MISC);
            }

            string fileName = path.ToLower(); //Path.GetFileNameWithoutExtension(path);
            if (!luaContext.LoadedFiles.Contains(fileName))
            {
                luaContext.State.DoFile(path);
                luaContext.LoadedFiles.Add(path);
            }

            var fn = luaContext.State[entryPoint] as LuaFunction;
            if(fn == null)
                return new ScriptResult(ScriptExecutionResult.ENTRYPOINT_NOT_FOUND);

            var ret = fn.Call(GetScriptIniter(context));
            var res = new ScriptResult(ScriptExecutionResult.SUCCESS);
            res.HasReturn = true; // who needs this anyway?
            res.Return = ret;
            return res;
        }

        public class LuaContext
        {
            public Lua State { get; set; }
            public readonly List<string> LoadedFiles = new List<string>();
        }

        public override void RegisterType(string name, Type type, IScriptContext context)
        {
            //var lua = (Lua) context.BindingsObj;
            //not required:
            //NLua has support for import('namespace')
        }
    }
}
