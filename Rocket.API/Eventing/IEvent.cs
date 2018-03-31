namespace Rocket.API.Eventing
{
    public interface IEvent
    {
        IEventEmitter Sender { get; }

        string Name { get; }

        bool IsAsync { get; }
    }
}