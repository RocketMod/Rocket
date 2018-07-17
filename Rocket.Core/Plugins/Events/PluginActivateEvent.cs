using Rocket.API.Eventing;
using Rocket.API.Plugins;

namespace Rocket.Core.Plugins.Events
{
    public class PluginActivateEvent : PluginEvent, ICancellableEvent
    {
        public PluginActivateEvent(IPluginLoader pluginLoader, IPlugin plugin) : base(pluginLoader, plugin) { }

        public PluginActivateEvent(IPluginLoader pluginLoader, IPlugin plugin, bool global = true) : base(
            pluginLoader,
            plugin, global) { }

        public PluginActivateEvent(IPluginLoader pluginLoader, IPlugin plugin,
                                   EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                   bool global = true) : base(pluginLoader, plugin, executionTarget, global) { }

        public bool IsCancelled { get; set; }
    }
}