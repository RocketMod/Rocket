using Rocket.API.Chat;
using System.Collections.ObjectModel;

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

        IChat Chat { get; }

        ReadOnlyCollection<IRocketPlayer> GetPlayers();

        void Shutdown();

        string InstanceId { get; }
        string Name { get; }

        void Reload();
    }
}