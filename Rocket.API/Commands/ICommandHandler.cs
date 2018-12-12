using System.Threading.Tasks;
using Rocket.API.DependencyInjection;
using Rocket.API.Player;
using Rocket.API.User;

namespace Rocket.API.Commands
{
    /// <summary>
    ///     The service responsible for handling commands.
    /// </summary>
    public interface ICommandHandler : IProxyableService, IService
    {
        /// <summary>
        ///     Handles a command by finding and executing the correct ICommand instance.
        /// </summary>
        /// <param name="user">
        ///     The <see cref="IUser">command user</see> of the command. Guaranteed to be a
        ///     <see cref="SupportsUser">supported user</see>.
        /// </param>
        /// <param name="commandLine">The command line (e.g. "/mycommand sub").</param>
        /// <param name="prefix">The <see cref="ICommandContext.CommandPrefix">prefix</see> of the command.</param>
        /// <returns><b>true</b> if the command was handled; otherwise, <b>false</b>.</returns>
        /// <remarks>
        ///     When returning false, the next command handler will be called.
        ///     If no command handler could handle the command, it will be assumed that the command does not exist.
        /// </remarks>
        Task<bool> HandleCommandAsync(IUser user, string commandLine, string prefix);

        /// <summary>
        ///     Defines if this command handler can handle the given command user type.
        /// </summary>
        /// <param name="user">The <see cref="IUser">command user</see> type to check.</param>
        /// <returns><b>true</b> if the command user type is supported; otherwise, <b>false</b>.</returns>
        bool SupportsUser(IUser user);

        /// <summary>
        ///     The permission required to execute the command.
        ///     <para>
        ///         <b>This method can return null.</b>
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The default CommandHandler uses {CommandName}.{SubCommand} (e.g. "Heal") as permission.
        ///     </para>
        /// </remarks>
        /// <example>
        ///     <c>"MyPermission.Heal"</c>
        /// </example>
        string GetPermission(ICommandContext context);
    }
}