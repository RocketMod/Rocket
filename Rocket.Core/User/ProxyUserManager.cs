using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Rocket.API.DependencyInjection;
using Rocket.API.User;

namespace Rocket.Core.User
{
    public class ProxyUserManager : IUserManager, IServiceProxy<IUserManager>
    {
        private readonly IDependencyContainer container;

        public ProxyUserManager(IDependencyContainer container)
        {
            this.container = container;
        }

        public IEnumerable<IUser> Users => ProxiedServices.SelectMany(c => c.Users);

        public IEnumerable<IUserManager> ProxiedServices => container.ResolveAll<IUserManager>();

        public bool Ban(IUserInfo user, IUser bannedBy = null, string reason = null, TimeSpan? timeSpan = null)
        {
            return user.UserManager.Ban(user, bannedBy, reason, timeSpan);
        }

        public void Broadcast(IUser sender, string message, Color? color = null, params object[] arguments)
        {
            throw new NotImplementedException();
        }

        public bool Kick(IUser user, IUser kickedBy = null, string reason = null)
        {
            return user.UserManager.Kick(user, kickedBy, reason);
        }

        public void SendMessage(IUser sender, IUser receiver, string message, params object[] arguments)
        {
            receiver.UserManager.SendMessage(sender, receiver, message, arguments);
        }

        public void SendMessage(IUser sender, IEnumerable<IUser> receivers, string message, params object[] arguments)
        {
            throw new NotImplementedException();
        }

        public void SendMessage(IUser sender, string message, params object[] arguments)
        {
            sender?.UserManager.SendMessage(sender, message, arguments);
        }

        public void SendMessage(IUser sender, string message, Color? color = null, params object[] arguments)
        {
            sender?.UserManager.SendMessage(sender, message, color, arguments);
        }

        public bool Unban(IUserInfo user, IUser unbannedBy = null)
        {
            return user.UserManager.Unban(user, unbannedBy);
        }
    }
}