using System;
using System.ComponentModel;
using Rocket.API.DependencyInjection;
using Rocket.API.Permissions;
using Rocket.API.Player;

namespace Rocket.Core.Player
{
    [TypeConverter(typeof(OnlinePlayerTypeConverter))]
    public abstract class BaseOnlinePlayer : BasePlayer, IOnlinePlayer
    {
        protected BaseOnlinePlayer(IDependencyContainer container) : base(container) { }

        public override string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return base.ToString(null, formatProvider);

            string[] subFormats = format.Split(':');

            format = subFormats[0];
            string subFormat = subFormats.Length > 1 ? subFormats[1] : null;

            if (format.Equals("group", StringComparison.OrdinalIgnoreCase))
                return Container.Get<IPermissionProvider>().GetPrimaryGroup(this).Name;

            if (!(this is ILivingEntity entity))
                return base.ToString(format, formatProvider);

            if (format.Equals("health", StringComparison.OrdinalIgnoreCase))
            {
                var health = entity.Health;
                return subFormat != null ? health.ToString(subFormat, formatProvider) : health.ToString(formatProvider);
            }

            if (format.Equals("maxhealth", StringComparison.OrdinalIgnoreCase))
            {
                var maxHealth = entity.MaxHealth;
                return subFormat != null ? maxHealth.ToString(subFormat, formatProvider) : maxHealth.ToString(formatProvider);
            }
            return base.ToString(format, formatProvider);
        }

        public abstract void SendMessage(string message, ConsoleColor? color = null);
        public abstract DateTime SessionConnectTime { get; }
        public abstract DateTime? SessionDisconnectTime { get; }
        public abstract TimeSpan SessionOnlineTime { get; }
    }
}