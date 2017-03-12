using System.Collections.Generic;
using Microsoft.ClearScript;
using Microsoft.ClearScript.V8;

namespace Rocket.Plugins.ClearScript.Impl
{
    public class JavaScriptEngine : ClearScriptEngine
    {
        public override string Name => "JavaScript";
        public override List<string> FileTypes => new List<string> {"js", "javascript"};
        public override string ShortName => "js";
        protected override ScriptEngine CreateNewEngine()
        {
            return new V8ScriptEngine();
        }
    }
}