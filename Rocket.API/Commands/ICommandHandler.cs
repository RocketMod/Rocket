
using Rocket.API.DependencyInjection;
using Rocket.API.ServiceProxies;

namespace Rocket.API.Commands
{
    public interface ICommandHandler : IProxyableService
    {
        /// <summary>
        ///     Handles a command
        /// </summary>
        /// <param name="caller">The caller</param>
        /// <param name="commandLine">The command line</param>
        /// <returns>true if the command was handled, false when not</returns>
        bool HandleCommand(ICommandCaller caller, string commandLine);

        bool SupportsCaller(ICommandCaller caller);

        /// <summary>
        ///    Get the command for the context
        /// </summary>
        /// <param name="ctx">The context</param>
        /// <returns>The command of the context</returns>
        ICommand GetCommand(ICommandContext ctx);
    }
}