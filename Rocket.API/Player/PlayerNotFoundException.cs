using System;

namespace Rocket.API.Player
{
    public sealed class PlayerNotFoundException : Exception
    {
        public string Name { get; private set; } = null;
        public string ID { get; private set; } = null;

        public static PlayerNotFoundException ThrowByName(string name)
        {
            PlayerNotFoundException ex = new PlayerNotFoundException();

            ex.Name = name;

            return ex;
        }

        public static PlayerNotFoundException ThrowByID(string id)
        {
            PlayerNotFoundException ex = new PlayerNotFoundException();

            ex.ID = id;

            return ex;
        }
    }
}
