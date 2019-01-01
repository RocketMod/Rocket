using Rocket.API.User;

namespace Rocket.API.Player
{
    public interface IPlayerUser<TPlayer> : IUser where TPlayer: IPlayer
    {
        TPlayer Player { get; }
    }
}