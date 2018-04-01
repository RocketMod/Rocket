namespace Rocket.API.Eventing
{
    public interface ICancellableEvent
    {
        bool IsCancelled { get; set; }
    }
}