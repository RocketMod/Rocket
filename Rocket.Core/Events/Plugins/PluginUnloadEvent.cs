using Rocket.API.Eventing;
using Rocket.API.Plugin;

namespace Rocket.Core.Events.Plugins
{
    public class PluginUnloadEvent : PluginEvent
    {
        public PluginUnloadEvent(IPlugin plugin, bool global = true) : base(plugin, global) { }
        public PluginUnloadEvent(IPlugin plugin, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(plugin, executionTarget, global) { }
        public PluginUnloadEvent(IPlugin plugin, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(plugin, name, executionTarget, global) { }

        public PluginUnloadEvent(IPlugin plugin) : base(plugin) { }
    }
}