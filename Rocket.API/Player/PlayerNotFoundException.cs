using System;

namespace Rocket.API.Player
{
    public sealed class PlayerNotFoundException : Exception
    {
        public readonly string Name;
        public readonly string ID;

        public PlayerNotFoundException(string id = null, string name = null) : base("The requested plasyer couldn't be found.")
        {
            ID = id;
            Name = name;
        }
    }
}
