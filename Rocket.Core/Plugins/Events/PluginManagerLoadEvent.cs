using Rocket.API.Eventing;
using Rocket.API.Plugin;
using Rocket.Core.Plugins;

namespace Rocket.Core.Plugins.Events
{
    /// <summary>
    /// This event is before plugin were loaded but after dependencies were set up.
    /// It should not be used by plugins itself, only by implementations.<br/><br/>
    /// Plugins can use <see cref="PluginLoadEvent"/> and <see cref="PluginLoadedEvent"/>
    /// </summary>
    public class PluginManagerInitEvent : Event, ICancellableEvent
    {
        public IPluginManager PluginManager { get; }

        public PluginManagerInitEvent(IPluginManager pluginManager, bool global = true) : base(global)
        {
            PluginManager = pluginManager;
        }
        public PluginManagerInitEvent(IPluginManager pluginManager, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(executionTarget, global)
        {
            PluginManager = pluginManager;
        }
        public PluginManagerInitEvent(IPluginManager pluginManager, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(name, executionTarget, global)
        {
            PluginManager = pluginManager;
        }
        public PluginManagerInitEvent(IPluginManager pluginManager) : this(pluginManager, true)
        {
            PluginManager = pluginManager;
        }

        public bool IsCancelled { get; set; }
    }
}