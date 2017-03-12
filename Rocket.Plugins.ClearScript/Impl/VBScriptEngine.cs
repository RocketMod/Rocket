using System.Collections.Generic;
using Microsoft.ClearScript;

namespace Rocket.Plugins.ClearScript.Impl
{
    //Another dead language but implementation is easy anyway again
    //might be Windows only?
    public class VBScriptEngine : ClearScriptEngine
    {
        public override string Name => "VBScript";
        public override List<string> FileTypes => new List<string> {"vbs"};
        public override string ShortName => "VBS";
        protected override ScriptEngine CreateNewEngine()
        {
            return new Microsoft.ClearScript.Windows.VBScriptEngine();
        }
    }
}