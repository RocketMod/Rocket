using Rocket.API.Plugins;

namespace Rocket.API.Event.Plugin
{
    public class PluginLoadingEvent : PluginEvent, ICancellableEvent
    {
        public PluginLoadingEvent(IRocketPlugin plugin) : base(plugin)
        {
        }

        public bool IsCancelled { get; set; }
    }
}