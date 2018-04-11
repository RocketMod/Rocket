using Rocket.API.Eventing;
using Rocket.API.Plugin;

namespace Rocket.Core.Events.Plugins
{
    public abstract class PluginEvent : Event
    {
        public IPluginManager PluginManager { get; }
        public IPlugin Plugin { get; }

        protected PluginEvent(IPluginManager pluginManager, IPlugin plugin, bool global = true) : base(global)
        {
            PluginManager = pluginManager;
            Plugin = plugin;
        }

        protected PluginEvent(IPluginManager pluginManager, IPlugin plugin,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(executionTarget, global)
        {
            PluginManager = pluginManager;
            Plugin = plugin;
        }

        protected PluginEvent(IPluginManager pluginManager, IPlugin plugin, string name = null,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(name, executionTarget, global)
        {
            Plugin = plugin;
        }

        protected PluginEvent(IPluginManager pluginManager, IPlugin plugin) : this(pluginManager, plugin, true) { }
    }
}