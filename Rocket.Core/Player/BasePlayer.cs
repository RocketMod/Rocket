using System;
using System.ComponentModel;
using Rocket.API.DependencyInjection;
using Rocket.API.Permissions;
using Rocket.API.Player;

namespace Rocket.Core.Player
{
    [TypeConverter(typeof(PlayerTypeConverter))]
    public abstract class BasePlayer : IPlayer
    {
        protected IDependencyContainer Container { get; }

        protected BasePlayer(IDependencyContainer container)
        {
            Container = container;
        }

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
        public abstract string Id { get; }
        public abstract string Name { get; }
        public abstract Type CallerType { get; }

        public abstract bool IsOnline { get; }

        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return Name.ToString(formatProvider); ;

            if (format.Equals("id", StringComparison.OrdinalIgnoreCase))
                return Id.ToString(formatProvider);

            if (format.Equals("name", StringComparison.OrdinalIgnoreCase))
                return Name.ToString(formatProvider);

            throw new FormatException($"\"{format}\" is not a valid format.");
        }
    }
}