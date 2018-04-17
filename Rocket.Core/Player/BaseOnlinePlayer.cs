using System;
using Rocket.API.DependencyInjection;
using Rocket.API.Permissions;
using Rocket.API.Player;

namespace Rocket.Core.Player
{
    public abstract class BaseOnlinePlayer : BasePlayer, IOnlinePlayer
    {
        protected BaseOnlinePlayer(IDependencyContainer container) : base(container) { }

        public abstract double Health { get; set; }
        public abstract double MaxHealth { get; set; }

        public override string ToString(string format, IFormatProvider formatProvider)
        {
            if (format != null)
            {
                string[] subFormats = format.Split(':');

                format = subFormats[0];
                string subFormat = subFormats.Length > 1 ? subFormats[1] : null;

                if (format.Equals("group", StringComparison.OrdinalIgnoreCase))
                    return Container.Get<IPermissionProvider>().GetPrimaryGroup(this).Name;
                
                if (format.Equals("health", StringComparison.OrdinalIgnoreCase))
                {
                    return subFormat != null ? Health.ToString(subFormat, formatProvider) : Health.ToString(formatProvider);
                }

                if (format.Equals("maxhealth", StringComparison.OrdinalIgnoreCase))
                {
                    return subFormat != null ? MaxHealth.ToString(subFormat, formatProvider) : MaxHealth.ToString(formatProvider);
                }
            }
            return base.ToString(format, formatProvider);
        }

        public abstract void SendMessage(string message, ConsoleColor? color = null);
    }
}