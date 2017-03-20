using Rocket.API.Commands;

namespace Rocket.Plugins.ScriptBase
{
    /// <summary>
    /// Passed to the main function of scripts to make things easier
    /// </summary>
    public class ScriptInitHelper
    {
        public IScriptContext Context;

        public ScriptEngine Engine { get; }

        public ScriptInitHelper(ScriptEngine engine, IScriptContext ctx)
        {
            Engine = engine;
            Context = ctx;
        }

        public void RegisterCommand(IRocketCommand command)
        {
            //todo
            //Engine.PluginProvider.Commands.Add(command);
        }

        public void RegisterCommand(RegisteredRocketCommand callback)
        {
            //todo
            //Engine.PluginProvider.Commands.Add(callback);
        }
    }
}