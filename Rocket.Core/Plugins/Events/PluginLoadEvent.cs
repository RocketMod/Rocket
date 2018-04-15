using Rocket.API.Eventing;
using Rocket.API.Plugin;

namespace Rocket.Core.Plugins.Events
{
    public class PluginLoadEvent : PluginEvent, ICancellableEvent
    {
        public PluginLoadEvent(IPluginManager pluginManager, IPlugin plugin) : base(pluginManager, plugin) { }
        public PluginLoadEvent(IPluginManager pluginManager, IPlugin plugin, bool global = true) : base(pluginManager, plugin, global) { }

        public PluginLoadEvent(IPluginManager pluginManager, IPlugin plugin,
                               EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                               bool global = true) : base(pluginManager, plugin, executionTarget, global) { }

        public PluginLoadEvent(IPluginManager pluginManager, IPlugin plugin, string name = null,
                               EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                               bool global = true) : base(pluginManager, plugin, name, executionTarget, global) { }

        public bool IsCancelled { get; set; }
    }
}