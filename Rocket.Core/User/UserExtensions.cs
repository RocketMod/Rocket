using System;
using System.Drawing;
using Rocket.API.User;
using System.Collections.Generic;

namespace Rocket.Core.User
{
    public static class UserExtensions
    {
        public static void SendMessage(this IUser user, string message, params object[] bindings)
        {
            SendMessage(user, message, null, bindings);
        }

        public static void SendMessage(this IUser user, string message, Color? color = null, params object[] bindings)
        {
            user.UserManager.SendMessage(null, message, color, bindings);
        }

        public static TimeSpan GetOnlineTime(this IUser user) 
            => (user.SessionDisconnectTime ?? DateTime.Now) - user.SessionConnectTime;

        public static void SendMessage(this IUserManager manager, IUser sender, IUser receiver, string message,
                                       params object[] arguments)
        {
            manager.SendMessage(sender, receiver, message, null, arguments);
        }

        public static void Broadcast(this IUserManager manager, IUser sender, IEnumerable<IUser> receivers,
                                     string message, params object[] arguments)
        {
            manager.Broadcast(sender, receivers, message, null, arguments);
        }

        public static void Broadcast(this IUserManager manager, IUser sender, string message, params object[] arguments)
        {
            manager.Broadcast(sender, message, null, arguments);
        }

        public static void SendMessage(this IUserManager manager, IUser receiver, string message, Color? color = null,
                                       params object[] arguments)
        {
            manager.SendMessage(null, receiver, message, color, arguments);

        }

        public static void SendMessage(this IUserManager manager, IUser receiver, string message, params object[] arguments)
        {
            manager.SendMessage(null, receiver, message, null, arguments);

        }

    }
}