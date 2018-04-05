using Rocket.API.Handlers;

namespace Rocket.API.Commands
{
    public interface ICommandHandler : IHandler
    {
        /// <summary>
        ///     Handles a command
        /// </summary>
        /// <param name="caller">The caller</param>
        /// <param name="commandLine">The command line</param>
        /// <returns>true if the command was handled, false when not</returns>
        bool HandleCommand(ICommandCaller caller, string commandLine);

        ICommand GetCommand(ICommandContext ctx);
    }
}