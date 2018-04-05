using System;

namespace Rocket.API.Player
{
    public sealed class PlayerNotFoundException : Exception
    {
        public PlayerNotFoundException(string player)
            : base(string.IsNullOrEmpty(player)
                ? "The requested player couldn't be found."
                : $"The requested player: \"{player}\" couldn't be found.")
        {
            Player = player;
        }

        public string Player { get; }
    }
}