using System;
using Rocket.API.Commands;
using Rocket.API.Player;

namespace Rocket.Core.Commands
{
    public class DefaultCommandExceptionHandler : ICommandExceptionHandler
    {
        public bool HandleException(ICommandContext ctx, Exception e)
        {
            if (e is PlayerNotFoundException)
            {
                ctx.Caller.SendMessage(e.Message);
                return true;
            }

            return false;
        }
    }
}