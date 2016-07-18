namespace Rocket.API
{
    public delegate void ImplementationShutdown();

    public interface IRocketImplementationEvents
    {
        event ImplementationShutdown OnShutdown;
    }
}