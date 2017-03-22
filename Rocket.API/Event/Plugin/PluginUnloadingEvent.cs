using Rocket.API.Providers.Plugins;

namespace Rocket.API.Event.Plugin
{
    public class PluginUnloadingEvent : PluginEvent
    {
        public PluginUnloadingEvent(IRocketPlugin plugin) : base(plugin)
        {
        }
    }
}