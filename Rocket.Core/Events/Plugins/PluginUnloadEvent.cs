using Rocket.API.Eventing;
using Rocket.API.Plugin;

namespace Rocket.Core.Events.Plugins
{
    public class PluginUnloadEvent : PluginEvent
    {
        public PluginUnloadEvent(IPluginManager plugnManager, IPlugin plugin, bool global = true) : base(plugnManager, plugin, global) { }

        public PluginUnloadEvent(IPluginManager plugnManager, IPlugin plugin,
                                 EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                 bool global = true) : base(plugnManager, plugin, executionTarget, global) { }

        public PluginUnloadEvent(IPluginManager plugnManager, IPlugin plugin, string name = null,
                                 EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                 bool global = true) : base(plugnManager, plugin, name, executionTarget, global) { }

        public PluginUnloadEvent(IPluginManager plugnManager, IPlugin plugin) : base(plugnManager, plugin) { }
    }
}