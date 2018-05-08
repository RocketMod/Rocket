using System;
using Rocket.API.DependencyInjection;
using Rocket.API.User;

namespace Rocket.API.Commands
{
    /// <summary>
    ///     The service responsible for handling commands.
    /// </summary>
    public interface ICommandHandler : IProxyableService
    {
        /// <summary>
        ///     Handles a command
        /// </summary>
        /// <param name="caller">
        ///     The <see cref="IUser">command caller</see> of the command. Guaranteed to be a
        ///     <see cref="SupportsCaller">supported command caller</see>.
        /// </param>
        /// <param name="commandLine">The command line (e.g. "/mycommand sub").</param>
        /// <param name="prefix">The <see cref="ICommandContext.CommandPrefix">prefix</see> of the command.</param>
        /// <returns><b>true</b> if the command was handled; otherwise, <b>false</b>.</returns>
        /// <remarks>
        ///     When returning false, the next command handler will be called.
        ///     If no command handler could handle the command, it will be assumed that the command does not exist.
        /// </remarks>
        bool HandleCommand(IUser caller, string commandLine, string prefix);

        /// <summary>
        ///     Defines if this command handler can handle the given command caller type.
        /// </summary>
        /// <param name="User">The <see cref="IUser">command caller</see> type to check.</param>
        /// <returns><b>true</b> if the command caller type is supported; otherwise, <b>false</b>.</returns>
        bool SupportsCaller(Type User);
    }
}