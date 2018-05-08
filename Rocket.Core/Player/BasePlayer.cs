using System;
using System.ComponentModel;
using Rocket.API.DependencyInjection;
using Rocket.API.Entities;
using Rocket.API.Permissions;
using Rocket.API.Player;
using Rocket.API.User;

namespace Rocket.Core.Player
{
    [TypeConverter(typeof(PlayerTypeConverter))]
    public abstract class BasePlayer : IPlayer, IFormattable
    {
        protected BasePlayer(IDependencyContainer container)
        {
            Container = container;
        }

        protected IDependencyContainer Container { get; }

        public abstract DateTime? LastSeen { get; }

        public abstract DateTime SessionConnectTime { get; }
        public abstract DateTime? SessionDisconnectTime { get; }
        public abstract TimeSpan SessionOnlineTime { get; }

        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return Name.ToString(formatProvider);

            if (format.Equals("id", StringComparison.OrdinalIgnoreCase))
                return Id.ToString(formatProvider);

            if (format.Equals("name", StringComparison.OrdinalIgnoreCase))
                return Name.ToString(formatProvider);

            string[] subFormats = format.Split(':');

            format = subFormats[0];
            string subFormat = subFormats.Length > 1 ? subFormats[1] : null;

            if (format.Equals("group", StringComparison.OrdinalIgnoreCase))
                return Container.Resolve<IPermissionProvider>().GetPrimaryGroup(User).Name;

            if (IsOnline && Entity is ILivingEntity entity)
            {
                if (format.Equals("health", StringComparison.OrdinalIgnoreCase))
                {
                    double health = entity.Health;
                    return subFormat != null
                        ? health.ToString(subFormat, formatProvider)
                        : health.ToString(formatProvider);
                }

                if (format.Equals("maxhealth", StringComparison.OrdinalIgnoreCase))
                {
                    double maxHealth = entity.MaxHealth;
                    return subFormat != null
                        ? maxHealth.ToString(subFormat, formatProvider)
                        : maxHealth.ToString(formatProvider);
                }
            }

            throw new FormatException($"\"{format}\" is not a valid format.");
        }

        public abstract string Id { get; }
        public abstract string Name { get; }
        public IdentityType Type => IdentityType.Player;
        public abstract IUser User { get; }
        public abstract IEntity Entity { get; }
        public abstract bool IsOnline { get; }
        public abstract void SendMessage(string message, params object[] arguments);
    }
}