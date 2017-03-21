using Rocket.API.Plugins;

namespace Rocket.API.Event.Plugin
{
    public class PluginLoadedEvent : PluginEvent
    {
        public PluginLoadedEvent(IRocketPlugin plugin) : base(plugin)
        {
        }
    }
}