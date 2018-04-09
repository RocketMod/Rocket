using Rocket.API.Eventing;
using Rocket.API.Plugin;

namespace Rocket.Core.Events.Plugins
{
    public class PluginLoadedEvent : PluginEvent
    {
        public PluginLoadedEvent(IPlugin plugin, bool global = true) : base(plugin, global) { }

        public PluginLoadedEvent(IPlugin plugin,
                                 EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                 bool global = true) : base(plugin, executionTarget, global) { }

        public PluginLoadedEvent(IPlugin plugin, string name = null,
                                 EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                 bool global = true) : base(plugin, name, executionTarget, global) { }

        public PluginLoadedEvent(IPlugin plugin) : base(plugin) { }
    }
}