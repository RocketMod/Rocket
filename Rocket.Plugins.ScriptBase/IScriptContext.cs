using Rocket.API.Plugins;

namespace Rocket.Plugins.ScriptBase
{
    /// <summary>
    /// A script context implementation with generic bindings support
    /// <see cref="IScriptContext"/>
    /// </summary>
    /// <typeparam name="T">The bindings type</typeparam>
    public class ScriptContext<T> : IScriptContext
    {
        public ScriptContext(ScriptRocketPlugin plugin, ScriptEngine scriptEngine, T bindings)
        {
            Plugin = plugin;
            ScriptEngine = scriptEngine;
            Bindings = bindings;
        }

        public ScriptRocketPlugin Plugin { get; }
        public ScriptEngine ScriptEngine { get; }
        /// <summary>
        /// The generic bindings
        /// </summary>
        public T Bindings { get; }
        public object BindingsObj => Bindings;
    }

    /// <summary>
    /// <p>The context of the script.</p>
    /// <p>Contains global variables, bindings, static instances etc...</p>
    /// </summary>
    public interface IScriptContext
    {
        /// <summary>
        /// <p>The rocket plugin associated with the script.</p>
        /// <p><b>Can be null if there is no associated plugin</b></p>
        /// </summary>
        ScriptRocketPlugin Plugin { get; }

        /// <summary>
        /// The script engine (scripting language implementation) associated with the script
        /// </summary>
        ScriptEngine ScriptEngine { get; }

        /// <summary>
        /// <p>Used to store bindings.</p>
        /// <p>Implementation details are up to scripting implementation</p>
        /// </summary>
        object BindingsObj { get; }
    }
}