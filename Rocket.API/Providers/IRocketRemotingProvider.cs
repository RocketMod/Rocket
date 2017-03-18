using Rocket.API;
using Rocket.API.Commands;
using Rocket.API.Providers;

namespace Rocket.Core
{
    public delegate void RockedCommandExecute(IRocketPlayer player, IRocketCommand command, ref bool cancel);

    public interface IRocketRemotingProvider : IRocketProviderBase
    {
        RockedCommandExecute OnExecute();
    }
}