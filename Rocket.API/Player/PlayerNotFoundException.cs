using System;

namespace Rocket.API.Player
{
    public sealed class PlayerNotFoundException : Exception
    {
        public string Name { get; }
        public string ID { get; }

        public PlayerNotFoundException(string id = null, string name = null) : base("The requested player couldn't be found.")
        {
            ID = id;
            Name = name;

            new PlayerNotFoundException();
        }
    }
}
