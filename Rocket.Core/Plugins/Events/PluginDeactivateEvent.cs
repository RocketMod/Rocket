using Rocket.API.Eventing;
using Rocket.API.Plugins;

namespace Rocket.Core.Plugins.Events
{
    public class PluginDeactivateEvent : PluginEvent
    {
        public PluginDeactivateEvent(IPluginLoader plugnLoader, IPlugin plugin, bool global = true) : base(
            plugnLoader,
            plugin, global) { }

        public PluginDeactivateEvent(IPluginLoader plugnLoader, IPlugin plugin,
                                     EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                     bool global = true) : base(plugnLoader, plugin, executionTarget, global) { }

        public PluginDeactivateEvent(IPluginLoader plugnLoader, IPlugin plugin) : base(plugnLoader, plugin) { }
    }
}