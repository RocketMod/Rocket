using Rocket.API.Eventing;
using Rocket.API.Plugins;

namespace Rocket.Core.Plugins.Events
{
    public class PluginDeactivateEvent : PluginEvent
    {
        public PluginDeactivateEvent(IPluginManager plugnManager, IPlugin plugin, bool global = true) : base(
            plugnManager,
            plugin, global) { }

        public PluginDeactivateEvent(IPluginManager plugnManager, IPlugin plugin,
                                     EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                     bool global = true) : base(plugnManager, plugin, executionTarget, global) { }

        public PluginDeactivateEvent(IPluginManager plugnManager, IPlugin plugin, string name = null,
                                     EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                     bool global = true) : base(plugnManager, plugin, name, executionTarget, global) { }

        public PluginDeactivateEvent(IPluginManager plugnManager, IPlugin plugin) : base(plugnManager, plugin) { }
    }
}