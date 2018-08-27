using System;
using System.Collections.Generic;
using Rocket.API.Drawing;
using System.Linq;
using Rocket.API.DependencyInjection;
using Rocket.API.User;
using Rocket.Core.ServiceProxies;
using Rocket.API.Player;

namespace Rocket.Core.User
{
    public class UserManagerProxy : ServiceProxy<IUserManager>, IUserManager
    {
        private readonly IDependencyContainer container;

        public UserManagerProxy(IDependencyContainer container) : base(container)
        {
            this.container = container;
        }

        public bool Ban(IUser user, IUser bannedBy = null, string reason = null, TimeSpan? timeSpan = null)
        {
            return user.UserManager.Ban(user, bannedBy, reason, timeSpan);
        }

        public bool Kick(IPlayer player, IUser kickedBy = null, string reason = null)
        {
            return player.PlayerManager.Kick(player, kickedBy, reason);
        }

        public void SendMessage(IUser sender, IPlayer receiver, string message, Color? color = null, params object[] arguments)
        {
            receiver.PlayerManager.SendMessage(sender, receiver, message, color, arguments);
        }

        public void Broadcast(IUser sender, IEnumerable<IPlayer> receivers, string message, Color? color = null, params object[] arguments)
        {
            if (sender == null)
            {
                var rec = receivers.ToList();
                foreach (var service in ProxiedServices)
                    service.Broadcast(null, rec, message, color, arguments);
                return;
            }
            sender.UserManager.Broadcast(sender, receivers, message, color, arguments);
        }

        public void Broadcast(IUser sender, string message, Color? color = null, params object[] arguments)
        {
            if (sender == null)
            {
                foreach (var service in ProxiedServices)
                    service.Broadcast(null, message, color, arguments);
                return;
            }

            sender.UserManager.Broadcast(sender, message, color, arguments);
        }

        public IUser GetUser(string id, IdentityProvider identityProvider = IdentityProvider.Builtin)
        {
            throw new Exception("Not supported on proxy.");
        }

        public bool Unban(IUser user, IUser unbannedBy = null)
        {
            return user.UserManager.Unban(user, unbannedBy);
        }


        public string ServiceName => "ProxyUsers";
    }
}