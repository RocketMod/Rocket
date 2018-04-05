namespace Rocket.API.Eventing {
    public interface IEventEmitter : ILifecycleObject {
        string Name { get; }
    }
}