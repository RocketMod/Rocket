using Rocket.API.DependencyInjection;
using System.Drawing;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.ServiceProxies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocket.Core.User
{
    public class UserManagerProxy : ServiceProxy<IUserManager>, IUserManager
    {
        private readonly IDependencyContainer container;

        public UserManagerProxy(IDependencyContainer container) : base(container)
        {
            this.container = container;
        }

        public Task<bool> BanAsync(IUser user, IUser bannedBy = null, string reason = null, TimeSpan? timeSpan = null)
        {
            return user.UserManager.BanAsync(user, bannedBy, reason, timeSpan);
        }

        public Task<bool> KickAsync(IUser user, IUser kickedBy = null, string reason = null)
        {
            return user.UserManager.KickAsync(user, kickedBy, reason);
        }

        public async Task SendMessageAsync(IUser sender, IUser receiver, string message, Color? color = null, params object[] arguments)
        {
            if (receiver == null)
            {
                foreach (var mgr in ProxiedServices)
                {
                    await mgr.SendMessageAsync(sender, receiver, message, color, arguments);
                }

                return;
            }

            await receiver.UserManager.SendMessageAsync(sender, receiver, message, color, arguments);
        }

        public async Task BroadcastAsync(IUser sender, IEnumerable<IUser> receivers, string message, Color? color = null, params object[] arguments)
        {
            if (sender == null)
            {
                var rec = receivers.ToList();
                foreach (var service in ProxiedServices)
                    await service.BroadcastAsync(null, rec, message, color, arguments);
                return;
            }

            await sender.UserManager.BroadcastAsync(sender, receivers, message, color, arguments);
        }

        public async Task BroadcastAsync(IUser sender, string message, Color? color = null, params object[] arguments)
        {
            if (sender == null)
            {
                foreach (var service in ProxiedServices)
                    await service.BroadcastAsync(null, message, color, arguments);
                return;
            }

            await sender.UserManager.BroadcastAsync(sender, message, color, arguments);
        }

        public Task<IUser> GetUserAsync(string id)
        {
            throw new NotSupportedException();
        }

        public Task<bool> UnbanAsync(IUser user, IUser unbannedBy = null)
        {
            return user.UserManager.UnbanAsync(user, unbannedBy);
        }


        public string ServiceName => "ProxyUsers";

        public Task<IIdentity> GetIdentity(string id) 
            => throw new NotSupportedException();
    }
}