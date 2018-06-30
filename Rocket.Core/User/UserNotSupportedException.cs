using System;
using Rocket.API.Commands;
using Rocket.API.Drawing;

namespace Rocket.Core.User
{
    public class UserNotSupportedException : Exception, ICommandFriendlyException
    {
        public UserNotSupportedException() : base("You can not execute this action.")
        {
            
        }
        public UserNotSupportedException(string message) : base(message)
        {
            
        }

        public void SendErrorMessage(ICommandContext context)
        {
            context.User.SendMessage(Message, Color.Red);
        }
    }
}