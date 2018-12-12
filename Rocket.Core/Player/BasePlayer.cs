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
    public abstract class BasePlayer<TUser, TEntity> : IPlayer<TUser, TEntity> 
        where TEntity : IPlayerEntity
        where TUser : IUser
    {
        protected BasePlayer(IDependencyContainer container, IPlayerManager manager)
        {
            Container = container.CreateChildContainer();
            PlayerManager = manager;
        }

        public IDependencyContainer Container { get; }

        public IPlayerManager PlayerManager { get; }

        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return User.ToString();

            if (format.Equals("id", StringComparison.OrdinalIgnoreCase))
                return User.Id.ToString().ToString(formatProvider);

            if (format.Equals("name", StringComparison.OrdinalIgnoreCase))
                return User.UserName.ToString(formatProvider);

            string[] subFormats = format.Split(':');

            format = subFormats[0];
            string subFormat = subFormats.Length > 1 ? subFormats[1] : null;

            if (format.Equals("group", StringComparison.OrdinalIgnoreCase))
                return Container.Resolve<IPermissionProvider>().GetPrimaryGroupAsync(User).GetAwaiter().GetResult().Name;

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

        public abstract TEntity Entity { get; }
        
        public abstract TUser User { get; }

        public abstract bool IsOnline { get; }
        
        public abstract DateTime SessionConnectTime { get; }

        public abstract DateTime? SessionDisconnectTime { get; }

        IUser IPlayer.User =>  User;

        IPlayerEntity IPlayer.Entity => Entity;
    }
}