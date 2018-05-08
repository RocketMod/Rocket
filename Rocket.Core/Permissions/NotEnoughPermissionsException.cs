using System;
using System.Drawing;
using Rocket.API.Commands;
using Rocket.API.User;
using Rocket.Core.Commands;
using Rocket.Core.User;

namespace Rocket.Core.Permissions
{
    public class NotEnoughPermissionsException : Exception, ICommandFriendlyException
    {
        public NotEnoughPermissionsException(IUser user, string[] permissions) : this(user, permissions,
            "You don't have enough permissions to do that.") { }

        public NotEnoughPermissionsException(IUser user, string[] permissions, string friendlyErrorMessage)
        {
            User = user;
            Permissions = permissions;
            FriendlyErrorMessage = friendlyErrorMessage;
        }

        public NotEnoughPermissionsException(IUser user, string permission, string friendlyErrorMessage) :
            this(user, new[] {permission}, friendlyErrorMessage) { }

        public NotEnoughPermissionsException(IUser user, string permission) : this(user,
            new[] {permission}) { }

        public IUser User { get; }
        public string[] Permissions { get; }
        public string FriendlyErrorMessage { get; }

        public override string Message
        {
            get
            {
                string message = $"{User.Name} does not have the following permissions: ";
                message += Environment.NewLine;
                foreach (string perm in Permissions) message += "* " + perm + Environment.NewLine;

                return message;
            }
        }

        public void SendErrorMessage(ICommandContext context)
        {
            context.User.SendMessage(FriendlyErrorMessage, Color.Red);
        }
    }
}