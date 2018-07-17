using Rocket.API.Eventing;
using Rocket.API.Plugins;
using Rocket.Core.Eventing;

namespace Rocket.Core.Plugins.Events
{
    /// <summary>
    ///     This event is before plugin were loaded but after dependencies were set up.
    ///     It should not be used by plugins itself, only by implementations.<br /><br />
    ///     Plugins can use <see cref="PluginActivateEvent" /> and <see cref="PluginActivatedEvent" />
    /// </summary>
    public class PluginManagerInitEvent : Event, ICancellableEvent
    {
        public PluginManagerInitEvent(IPluginLoader pluginLoader, bool global = true) : base(global)
        {
            PluginLoader = pluginLoader;
        }

        public PluginManagerInitEvent(IPluginLoader pluginLoader,
                                      EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                      bool global = true) : base(executionTarget, global)
        {
            PluginLoader = pluginLoader;
        }

        public PluginManagerInitEvent(IPluginLoader pluginLoader) : this(pluginLoader, true)
        {
            PluginLoader = pluginLoader;
        }

        public IPluginLoader PluginLoader { get; }

        public bool IsCancelled { get; set; }
    }
}