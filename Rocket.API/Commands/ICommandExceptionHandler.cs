using System;
using Rocket.API.Handlers;

namespace Rocket.API.Commands
{
    public interface ICommandExceptionHandler : IHandler
    {
        bool HandleException(ICommandContext ctx, Exception e);
    }
}