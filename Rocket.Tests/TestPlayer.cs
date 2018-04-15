using System;
using Rocket.API.Permissions;
using Rocket.API.Player;

namespace Rocket.Tests
{
    public class TestPlayer : IPlayer
    {
        public int CompareTo(object obj) => CompareTo((IIdentifiable) obj);

        public int CompareTo(IIdentifiable other) => CompareTo(other.Id);

        public bool Equals(IIdentifiable other)
        {
            if (other == null)
                return false;

            return Equals(other.Id);
        }

        public int CompareTo(string other) => Id.CompareTo(other);

        public bool Equals(string other) => Id.Equals(other, StringComparison.OrdinalIgnoreCase);

        public string Id { get; set; }
        public string Name { get; set; }
        public Type CallerType => typeof(TestPlayer);

        public void SendMessage(string message)
        {
            throw new NotImplementedException();
        }
    }
}