using Rocket.API.Plugin;

namespace Rocket.API.Eventing
{
    public interface IEventManager
    {
        void RegisterEvents(IEventListener listener, IPlugin plugin);

        void UnregisterEvents(IEventListener listener, IPlugin plugin);

        void TriggerEvent(Event @event);

        void UnregisterAllEvents(IPlugin plugin);

        void RegisterAllEvents(IPlugin plugin);
    }
}