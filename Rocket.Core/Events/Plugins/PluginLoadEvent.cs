using Rocket.API.Eventing;
using Rocket.API.Plugin;

namespace Rocket.Core.Events.Plugins
{
    public class PluginLoadEvent : PluginEvent, ICancellableEvent
    {
        public PluginLoadEvent(IPlugin plugin) : base(plugin) { }
        public PluginLoadEvent(IPlugin plugin, bool global = true) : base(plugin, global) { }

        public PluginLoadEvent(IPlugin plugin,
                               EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                               bool global = true) : base(plugin, executionTarget, global) { }

        public PluginLoadEvent(IPlugin plugin, string name = null,
                               EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                               bool global = true) : base(plugin, name, executionTarget, global) { }

        public bool IsCancelled { get; set; }
    }
}