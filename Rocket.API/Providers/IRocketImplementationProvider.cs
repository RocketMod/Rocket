using Rocket.API.Implementation.Managers;

namespace Rocket.API.Providers
{
    public delegate void ImplementationInitialized();
    public delegate void ImplementationShutdown();
    public delegate void ImplementationReload();

    public interface IRocketImplementationProvider
    {
        string InstanceName { get; }
        string Name { get; }
        IChatManager Chat { get; }
        IPlayerManager Players { get; }
        IRocketCommandProvider CommandProvider { get; }

        event ImplementationInitialized OnInitialized;
        event ImplementationShutdown OnShutdown;
        event ImplementationReload OnReload;

        void Shutdown();
        void Reload();
    }
}