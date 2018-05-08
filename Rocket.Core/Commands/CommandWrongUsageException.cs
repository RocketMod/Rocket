using System;
using Rocket.API.Commands;

namespace Rocket.Core.Commands
{
    public class CommandWrongUsageException : Exception, ICommandFriendlyException
    {
        public CommandWrongUsageException() : base("The command was not used correctly.") { }

        public CommandWrongUsageException(string message) : base(message) { }

        public virtual void SendErrorMessage(ICommandContext context)
        {
            context.Caller.SendMessage(Message, ConsoleColor.DarkRed);
            context.SendHelpMessage();
        }
    }
}