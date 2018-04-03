using Rocket.API.Player;

namespace Rocket.API.Commands
{
    public interface ICommandContext
    {
        string Command { get; }

        ICommandCaller Caller { get; }

        string[] Parameters { get; }
    }
}
