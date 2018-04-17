using Rocket.API.Commands;

namespace Rocket.API.Player {
    public interface IOnlinePlayer : IPlayer, ICommandCaller
    {
        double Health { get; set; }
        double MaxHealth { get; set; }
    }
}