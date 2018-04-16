using System;
using Rocket.API.Permissions;
using Rocket.API.Player;

namespace Rocket.Core.Plugins
{
    public abstract class BasePlayer : IPlayer
    {
        public int CompareTo(object obj) => CompareTo((IIdentifiable)obj);

        public int CompareTo(IIdentifiable other) => CompareTo(other.Id);

        public bool Equals(IIdentifiable other)
        {
            if (other == null)
                return false;

            return Equals(other.Id);
        }

        public int CompareTo(string other) => Id.CompareTo(other);

        public bool Equals(string other) => Id.Equals(other, StringComparison.OrdinalIgnoreCase);


        public abstract string Id { get; protected set; }
        public abstract string Name { get; protected set; }
        public abstract Type CallerType { get; }
        public abstract void SendMessage(string message);

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return Name;

            if (format.Equals("id", StringComparison.OrdinalIgnoreCase))
                return Id;

            if (format.Equals("name", StringComparison.OrdinalIgnoreCase))
                return Name;

            throw new FormatException($"\"{format}\" is not a valid format.");
        }
    }
}