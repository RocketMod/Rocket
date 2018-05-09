using Rocket.API.Eventing;
using Rocket.API.Plugins;

namespace Rocket.Core.Plugins.Events
{
    public class PluginActivatedEvent : PluginEvent
    {
        public PluginActivatedEvent(IPluginManager pluginManager, IPlugin plugin, bool global = true) : base(
            pluginManager,
            plugin, global) { }

        public PluginActivatedEvent(IPluginManager pluginManager, IPlugin plugin,
                                    EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                    bool global = true) : base(pluginManager, plugin, executionTarget, global) { }

        public PluginActivatedEvent(IPluginManager pluginManager, IPlugin plugin, string name = null,
                                    EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                    bool global = true) : base(pluginManager, plugin, name, executionTarget, global) { }

        public PluginActivatedEvent(IPluginManager pluginManager, IPlugin plugin) : base(pluginManager, plugin) { }
    }
}