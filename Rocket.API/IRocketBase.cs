using Rocket.API.Assets;
using Rocket.API.Collections;
using Rocket.API.Commands;
using Rocket.API.Permissions;
using Rocket.API.Plugins;
using Rocket.Core.Serialization;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rocket.API
{
    public delegate void RockedInitialized();
    public delegate void RockedReload();
    public delegate void RockedCommandExecute(IRocketPlayer player, IRocketCommand command, ref bool cancel);

    public interface IRocketBase
    {
        event RockedInitialized OnInitialized;
        event RockedCommandExecute OnCommandExecute;

        IRocketImplementation Implementation { get; }
        IRocketPermissionsProvider Permissions { get; set; }
        List<IRocketPluginManager> PluginManagers { get; }

        ReadOnlyCollection<IRocketPlugin> GetPlugins();
        ReadOnlyCollection<IRocketCommand> GetCommands();
        IRocketCommand GetCommand(string name);

        XMLFileAsset<RocketSettings> Settings { get; }
        XMLFileAsset<TranslationList> Translation { get; }

        void Reload();
        bool Execute(IRocketPlayer caller, string commandString);
    }
}
