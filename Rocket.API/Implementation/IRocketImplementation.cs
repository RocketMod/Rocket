using Rocket.API.Assets;
using Rocket.API.Chat;
using Rocket.API.Collections;
using Rocket.API.Commands;
using Rocket.API.Serialisation;
using System.Collections.ObjectModel;

namespace Rocket.API
{
    public delegate void ImplementationInitialized();
    public delegate void ImplementationShutdown();
    public delegate void ImplementationReload();
    public delegate void PlayerConnected(IRocketPlayer player);
    public delegate void PlayerDisconnected(IRocketPlayer player);

    public interface IRocketImplementation
    {
        //XMLFileAsset<TranslationList> Translation { get; }
        IChat Chat { get; }
        string InstanceName { get; }
        string Name { get; }

        event ImplementationInitialized OnInitialized;
        event ImplementationShutdown OnShutdown;
        event ImplementationReload OnReload;
        event PlayerConnected OnPlayerConnected;
        event PlayerDisconnected OnPlayerDisconnected;

        ReadOnlyCollection<IRocketPlayer> GetAllPlayers();
        ReadOnlyCollection<IRocketCommand> GetAllCommands();

        void Shutdown();
        void Reload();
    }
}