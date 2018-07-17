using Rocket.API.Eventing;
using Rocket.API.Plugins;

namespace Rocket.Core.Plugins.Events
{
    public class PluginDeactivatedEvent : PluginEvent
    {
        public PluginDeactivatedEvent(IPluginLoader pluginLoader, IPlugin plugin, bool global = true) : base(
            pluginLoader, plugin, global) { }

        public PluginDeactivatedEvent(IPluginLoader pluginLoader, IPlugin plugin,
                                      EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                      bool global = true) : base(pluginLoader, plugin, executionTarget, global) { }

        public PluginDeactivatedEvent(IPluginLoader pluginLoader, IPlugin plugin) : base(pluginLoader, plugin) { }
    }
}