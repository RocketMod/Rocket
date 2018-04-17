namespace Rocket.API.Player {
    public interface IOnlinePlayer : IPlayer
    {
        double Health { get; set; }
        double MaxHealth { get; set; }
    }
}