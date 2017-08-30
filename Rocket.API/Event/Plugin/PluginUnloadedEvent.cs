
using Rocket.API.Plugins;

namespace Rocket.API.Event.Plugin
{
    public class PluginUnloadedEvent : PluginEvent
    {
        public PluginUnloadedEvent(IPlugin plugin) : base(plugin)
        {
        }
    }
}