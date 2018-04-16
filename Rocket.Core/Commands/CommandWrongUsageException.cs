using System;
using Rocket.API.Commands;
using Rocket.Core.Exceptions;

namespace Rocket.Core.Commands {
    public class CommandWrongUsageException : Exception, ICommandFriendlyException
    {
        public CommandWrongUsageException() : base("Command was used wrong") { }

        public CommandWrongUsageException(string message) : base(message) { }

        public virtual void SendErrorMessage(ICommandContext context)
        {
            context.Caller.SendMessage(Message, ConsoleColor.DarkRed);
            context.Caller.SendMessage("Correct usage:");
            context.Caller.SendMessage(context.CommandPrefix + context.CommandAlias + " " + context.Command.GetSyntax(context));
        }
    }
}