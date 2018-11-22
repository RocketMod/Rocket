using System;
using System.Threading.Tasks;
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

        public async Task SendErrorMessageAsync(ICommandContext context)
        {
            await context.User.SendMessageAsync(Message, Color.Red);
        }
    }
}