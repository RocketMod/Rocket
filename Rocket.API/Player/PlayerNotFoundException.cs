using System;

namespace Rocket.API.Player
{
    public class PlayerNotFoundException : Exception
    {
        public PlayerNotFoundException() : base($"Could not find the requested player.") { }
    }
}
