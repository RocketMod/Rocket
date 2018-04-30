using System;
using Rocket.API.Commands;
using Rocket.Core.Commands;

namespace Rocket.Core.Permissions
{
    public class NotEnoughPermissionsException : Exception, ICommandFriendlyException
    {
        public NotEnoughPermissionsException(ICommandCaller caller, string[] permissions) : this(caller, permissions,
            "You don't have enough permissions to do that.") { }

        public NotEnoughPermissionsException(ICommandCaller caller, string[] permissions, string friendlyErrorMessage)
        {
            Caller = caller;
            Permissions = permissions;
            FriendlyErrorMessage = friendlyErrorMessage;
        }

        public NotEnoughPermissionsException(ICommandCaller caller, string permission, string friendlyErrorMessage) :
            this(caller, new[] {permission}, friendlyErrorMessage) { }

        public NotEnoughPermissionsException(ICommandCaller caller, string permission) : this(caller,
            new[] {permission}) { }

        public ICommandCaller Caller { get; }
        public string[] Permissions { get; }
        public string FriendlyErrorMessage { get; }

        public override string Message
        {
            get
            {
                string message = $"{Caller.Name} does not have the following permissions: ";
                message += Environment.NewLine;
                foreach (string perm in Permissions) message += "* " + perm + Environment.NewLine;

                return message;
            }
        }

        public void SendErrorMessage(ICommandContext context)
        {
            context.Caller.SendMessage(FriendlyErrorMessage, ConsoleColor.Red);
        }
    }
}