using Rocket.API.Commands;
using Rocket.API.Player;

namespace Rocket.API.Providers
{
    public delegate void RockedCommandExecute(IRocketPlayer player, IRocketCommand command, ref bool cancel);

    public interface IRocketRemotingProvider : IRocketProviderBase
    {
        RockedCommandExecute OnExecute();
    }
}