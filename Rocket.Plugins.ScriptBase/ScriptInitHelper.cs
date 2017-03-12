using Rocket.API.Commands;

namespace Rocket.Plugins.ScriptBase
{
    /// <summary>
    /// Passed to the main function of scripts to make things easier
    /// </summary>
    public class ScriptInitHelper
    {
        public ScriptEngine Engine { get; } //todo: hide from scripts?

        public ScriptInitHelper(ScriptEngine engine)
        {
            Engine = engine;
        }

        public void RegisterCommand(IRocketCommand command)
        {
            Engine.PluginManager.Commands.Add(command);
        }

        public void RegisterCommand(RegisteredRocketCommand callback)
        {
            Engine.PluginManager.Commands.Add(callback);
        }
    }
}