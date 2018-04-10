using Rocket.API.Eventing;
using Rocket.API.Plugin;
using Rocket.Core.Plugins;

namespace Rocket.Core.Events.Plugins
{
    /// <summary>
    /// This event is before plugin were loaded but after dependencies were set up.
    /// It should not be used by plugins itself, only by implementations.<br/><br/>
    /// Plugins can use <see cref="PluginLoadEvent"/> and <see cref="PluginLoadedEvent"/>
    /// </summary>
    public class PluginManagerLoadEvent : Event
    {
        public IPluginManager PluginManager { get; }

        public PluginManagerLoadEvent(IPluginManager pluginManager, bool global = true) : base(global)
        {
            PluginManager = pluginManager;
        }
        public PluginManagerLoadEvent(IPluginManager pluginManager, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(executionTarget, global)
        {
            PluginManager = pluginManager;
        }
        public PluginManagerLoadEvent(IPluginManager pluginManager, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(name, executionTarget, global)
        {
            PluginManager = pluginManager;
        }
        public PluginManagerLoadEvent(IPluginManager pluginManager) : this(pluginManager, true)
        {
            PluginManager = pluginManager;
        }
    }
}