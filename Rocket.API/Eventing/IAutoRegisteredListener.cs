namespace Rocket.API.Eventing
{
    /// <summary>
    /// Types which implement this type will automatically get instantiated and registered.
    /// </summary>
    public interface IAutoRegisteredListener : IEventListener
    {
        
    }
}