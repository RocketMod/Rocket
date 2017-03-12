using System.Collections.Generic;
using Microsoft.ClearScript;

namespace Rocket.Plugins.ClearScript.Impl
{
    //Dead language but implementation is easy anyway
    //might be Windows only?
    public class JScriptEngine : ClearScriptEngine
    {
        public override string Name => "JScript";
        public override List<string> FileTypes => new List<string> {"js", "jscript"};
        public override string ShortName => "js";
        protected override ScriptEngine CreateNewEngine()
        {
            return new Microsoft.ClearScript.Windows.JScriptEngine();
        }
    }
}