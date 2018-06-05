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

        public bool Kick(IUser user, IUser kickedBy = null, string reason = null)
        {
            return user.UserManager.Kick(user, kickedBy, reason);
        }

        public void SendMessage(IUser sender, IUser receiver, string message, Color? color = null, params object[] arguments)
        {
            receiver.UserManager.SendMessage(sender, receiver, message, color, arguments);
        }

        public void Broadcast(IUser sender, IEnumerable<IUser> receivers, string message, Color? color = null, params object[] arguments)
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

        public IUserInfo GetUser(string id)
        {
            throw new Exception("Not supported on proxy.");
        }

        public bool Unban(IUserInfo user, IUser unbannedBy = null)
        {
            return user.UserManager.Unban(user, unbannedBy);
        }

        public string ServiceName => "ProxyUsers";
    }
}