using Rocket.API.Eventing;
using Rocket.API.Plugins;

namespace Rocket.Core.Plugins.Events
{
    public class PluginDeactivatedEvent : PluginEvent
    {
        public PluginDeactivatedEvent(IPluginManager pluginManager, IPlugin plugin, bool global = true) : base(
            pluginManager, plugin, global) { }

        public PluginDeactivatedEvent(IPluginManager pluginManager, IPlugin plugin,
                                      EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                      bool global = true) : base(pluginManager, plugin, executionTarget, global) { }

        public PluginDeactivatedEvent(IPluginManager pluginManager, IPlugin plugin, string name = null,
                                      EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                      bool global = true) : base(pluginManager, plugin, name, executionTarget,
            global) { }

        public PluginDeactivatedEvent(IPluginManager pluginManager, IPlugin plugin) : base(pluginManager, plugin) { }
    }
}