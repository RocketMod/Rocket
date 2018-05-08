using System;
using Rocket.API.Commands;
using Rocket.Core.User;

namespace Rocket.Core.Commands
{
    public class CommandWrongUsageException : Exception, ICommandFriendlyException
    {
        public CommandWrongUsageException() : base("The command was not used correctly.") { }

        public CommandWrongUsageException(string message) : base(message) { }

        public virtual void SendErrorMessage(ICommandContext context)
        {
            context.User.SendMessage(Message, ConsoleColor.DarkRed);
            context.SendCommandUsage();
        }
    }
}