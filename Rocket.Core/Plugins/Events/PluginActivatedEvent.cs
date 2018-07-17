using Rocket.API.Eventing;
using Rocket.API.Plugins;

namespace Rocket.Core.Plugins.Events
{
    public class PluginActivatedEvent : PluginEvent
    {
        public PluginActivatedEvent(IPluginLoader pluginLoader, IPlugin plugin, bool global = true) : base(
            pluginLoader,
            plugin, global) { }

        public PluginActivatedEvent(IPluginLoader pluginLoader, IPlugin plugin,
                                    EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                    bool global = true) : base(pluginLoader, plugin, executionTarget, global) { }

        public PluginActivatedEvent(IPluginLoader pluginLoader, IPlugin plugin) : base(pluginLoader, plugin) { }
    }
}