namespace Rocket.API
{
    public delegate void ImplementationInitialized();
    public delegate void ImplementationShutdown();
    public delegate void ImplementationReload();

    public interface IRocketImplementation
    {
        event ImplementationInitialized OnInitialized;
        event ImplementationShutdown OnShutdown;
        event ImplementationReload OnReload;

        void Shutdown();

        string InstanceId { get; }

        void Reload();
    }
}