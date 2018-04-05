namespace Rocket.API.Eventing {
    public interface IEventListener { }

    public interface IEventListener<in TEvent> : IEventListener where TEvent : IEvent {
        void HandleEvent(IEventEmitter emitter, TEvent @event);
    }
}