

using Rocket.API.Plugins;

namespace Rocket.API.Event.Plugin
{
    public abstract class PluginEvent : Event
    {
        public IPlugin Plugin { get; }

        protected PluginEvent(IPlugin plugin)
        {
            Plugin = plugin;
        }
    }
}