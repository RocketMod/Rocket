using Rocket.API.Player;

namespace Rocket.API.Commands
{
    public interface ICommandContext
    {
        IPlayer Caller { get; }

        ICommand Command { get; }

        string[] Parameters { get; }
    }
}
