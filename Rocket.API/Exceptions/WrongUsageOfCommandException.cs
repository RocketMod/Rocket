using Rocket.API.Commands;
using System;

namespace Rocket.API.Exceptions
{
    public class WrongUsageOfCommandException : Exception
    {
        public ICommandContext CommandContext { get; }


        public WrongUsageOfCommandException(ICommandContext ctx)
        {
            CommandContext = ctx;
        }

        public override string Message => "The player " + CommandContext.Caller.DisplayName + " did not correctly use the command " + CommandContext.Command.Name;
    }
}