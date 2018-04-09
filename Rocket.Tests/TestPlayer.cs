using System;
using Rocket.API.Permissions;
using Rocket.API.Player;

namespace Rocket.Tests
{
    public class TestPlayer : IPlayer
    {
        public int CompareTo(object obj)
        {
            return CompareTo((IIdentifiable) obj);
        }

        public int CompareTo(IIdentifiable other)
        {
            return CompareTo(other.Id);
        }

        public bool Equals(IIdentifiable other)
        {
            if (other == null)
                return false;

            return Equals(other.Id);
        }

        public int CompareTo(string other)
        {
            return Id.CompareTo(other);
        }

        public bool Equals(string other)
        {
            return Id.Equals(other, StringComparison.OrdinalIgnoreCase);
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public Type PlayerType => typeof(TestPlayer);

        public void SendMessage(string message)
        {
            throw new System.NotImplementedException();
        }
    }
}