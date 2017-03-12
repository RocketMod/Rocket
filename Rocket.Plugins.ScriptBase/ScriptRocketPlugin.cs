using Rocket.API;
using Rocket.API.Plugins;

namespace Rocket.Plugins.ScriptBase
{
    /// <summary>
    /// <p>The RocketPluginBase for a scripting implementation</p>
    /// <p>To be implemented by the script implementation.</p>
    /// </summary>
    public abstract class ScriptRocketPlugin : RocketPluginBase
    {
        public void Awake()
        {
            Name = GetType().Name;
            PluginManager = ScriptPluginManager;
            base.Initialize();
        }

        public abstract ScriptRocketPluginManager ScriptPluginManager { get; }
    }

    /// <summary>
    /// <p>The RocketPluginBase with configuration for a scripting implementation</p>
    /// <p>To be implemented by the script implementation.</p>
    /// </summary>
    public abstract class ScriptRocketPlugin<RocketPluginConfiguration> : RocketPluginBase<RocketPluginConfiguration> where RocketPluginConfiguration : class, IRocketPluginConfiguration
    {
        public void Awake()
        {
            Name = GetType().Name;
            PluginManager = ScriptPluginManager;
            base.Initialize();
        }

        public abstract ScriptRocketPluginManager ScriptPluginManager { get; }
    }
}