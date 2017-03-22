using Rocket.API.Providers.Plugins;

namespace Rocket.API.Event.Plugin
{
    public abstract class PluginEvent : Event
    {
        public IRocketPlugin Plugin { get; }

        protected PluginEvent(IRocketPlugin plugin)
        {
            Plugin = plugin;
        }
    }
}