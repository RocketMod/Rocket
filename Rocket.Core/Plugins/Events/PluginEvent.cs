using Rocket.API.Eventing;
using Rocket.API.Plugins;
using Rocket.Core.Eventing;

namespace Rocket.Core.Plugins.Events
{
    public abstract class PluginEvent : Event
    {
        protected PluginEvent(IPluginLoader pluginLoader, IPlugin plugin, bool global = true) : base(global)
        {
            PluginLoader = pluginLoader;
            Plugin = plugin;
        }

        protected PluginEvent(IPluginLoader pluginLoader, IPlugin plugin,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(executionTarget, global)
        {
            PluginLoader = pluginLoader;
            Plugin = plugin;
        }

        protected PluginEvent(IPluginLoader pluginLoader, IPlugin plugin) : this(pluginLoader, plugin, true) { }
        public IPluginLoader PluginLoader { get; }
        public IPlugin Plugin { get; }
    }
}