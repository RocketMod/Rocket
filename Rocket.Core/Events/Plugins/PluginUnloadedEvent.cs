using Rocket.API.Eventing;
using Rocket.API.Plugin;

namespace Rocket.Core.Events.Plugins
{
    public class PluginUnloadedEvent : PluginEvent
    {
        public PluginUnloadedEvent(IPlugin plugin, bool global = true) : base(plugin, global) { }

        public PluginUnloadedEvent(IPlugin plugin,
                                   EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                   bool global = true) : base(plugin, executionTarget, global) { }

        public PluginUnloadedEvent(IPlugin plugin, string name = null,
                                   EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                   bool global = true) : base(plugin, name, executionTarget, global) { }

        public PluginUnloadedEvent(IPlugin plugin) : base(plugin) { }
    }
}