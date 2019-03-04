using System;
using System.Threading.Tasks;
using System.Drawing;
using Rocket.API.Commands;
using Rocket.API.User;
using Rocket.Core.User;

namespace Rocket.Core.Permissions
{
    public class NotEnoughPermissionsException : Exception, ICommandFriendlyException
    {
        private readonly bool any;

        public NotEnoughPermissionsException(IUser user, string[] permissions, bool any = false) : this(user, permissions,
            "You don't have enough permissions to do that.", any)
        { }

        public NotEnoughPermissionsException(IUser user, string[] permissions, string friendlyErrorMessage, bool any = false)
        {
            this.any = any;
            User = user;
            Permissions = permissions;
            FriendlyErrorMessage = friendlyErrorMessage;
        }

        public NotEnoughPermissionsException(IUser user, string permission, string friendlyErrorMessage, bool any = false) :
            this(user, new[] { permission }, friendlyErrorMessage, any)
        { }

        public NotEnoughPermissionsException(IUser user, string permission, bool any = false) : this(user,
            new[] { permission }, any)
        { }

        public IUser User { get; }
        public string[] Permissions { get; }
        public string FriendlyErrorMessage { get; }

        public override string Message
        {
            get
            {
                string message = any 
                    ? $"{User.DisplayName} does not have any of the following permissions: " 
                    : $"{User.DisplayName} does not have the following permissions: ";

                message += Environment.NewLine;
                foreach (string perm in Permissions) message += "* " + perm + Environment.NewLine;

                return message;
            }
        }

        public async Task SendErrorMessageAsync(ICommandContext context)
        {
            await context.User.SendMessageAsync(FriendlyErrorMessage, Color.Red);
        }
    }
}