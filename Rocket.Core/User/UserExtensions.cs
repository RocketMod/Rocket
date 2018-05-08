using System.Drawing;
using Rocket.API.User;

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
    }
}