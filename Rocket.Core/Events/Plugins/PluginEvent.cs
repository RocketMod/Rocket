using Rocket.API.Eventing;
using Rocket.API.Plugin;

namespace Rocket.Core.Events.Plugins
{
    public abstract class PluginEvent : Event
    {
        public IPlugin Plugin { get; }

        protected PluginEvent(IPlugin plugin, bool global = true) : base(global)
        {
            Plugin = plugin;
        }

        protected PluginEvent(IPlugin plugin,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(executionTarget, global)
        {
            Plugin = plugin;
        }

        protected PluginEvent(IPlugin plugin, string name = null,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(name, executionTarget, global)
        {
            Plugin = plugin;
        }

        protected PluginEvent(IPlugin plugin) : this(plugin, true) { }
    }
}