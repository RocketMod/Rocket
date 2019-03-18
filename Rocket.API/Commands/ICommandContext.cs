using System.Threading.Tasks;
using Rocket.API.DependencyInjection;
using Rocket.API.Player;
using Rocket.API.User;

namespace Rocket.API.Commands
{
    /// <summary>
    ///     A command execution context. Includes various information about the environment the command was called in.
    /// </summary>
    public interface ICommandContext
    {
        /// <summary>
        ///     The parent command context for Child Commands.
        ///     <para>
        ///         <b>This property can return null.</b>
        ///     </para>
        /// </summary>
        /// <example>
        ///     If the command was entered as "/mycommand sub", this will return the parent context with parameters "sub".
        /// </example>
        ICommandContext ParentContext { get; }

        /// <summary>
        ///     The child command context.
        /// </summary>
        ICommandContext ChildContext { get; }

        /// <summary>
        ///     The root context.
        ///     <para>
        ///         <b>This property will never return null.</b>
        ///     </para>
        /// </summary>
        ICommandContext RootContext { get; }

        /// <summary>
        ///     <para>The prefix used to call this (sub) command.</para>
        ///     <para>Useful when sending command usage messages.</para>
        ///     <para>
        ///         <see cref="IChildCommand">Child Commands</see> include their parents.
        ///     </para>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <example>
        ///     <para>
        ///         If the command was executed using "/mycommand", it will be "/", when it was executed using "!mycommand", it
        ///         will be "!".
        ///     </para>
        ///     <para>
        ///         If the command was a ChildrenCommand "sub", "/mycommand sub" will return "/mycommand" as prefix.
        ///     </para>
        /// </example>
        string CommandPrefix { get; }

        /// <summary>
        ///     The alias or name used to execute this (sub) command.
        /// </summary>
        string CommandAlias { get; }

        /// <summary>
        ///     <para>The (sub) command associated with the context.</para>
        ///     <para>
        ///         <b>This property will never return null.</b>
        ///     </para>
        /// </summary>
        ICommand Command { get; }

        /// <summary>
        ///     <para>The user executing command.</para>
        ///     <para>Is guaranteed to be a <see cref="ICommand.SupportsUser">supported user</see>.</para>
        ///     <para>
        ///         <b>This property will never return null.</b>
        ///     </para>
        /// </summary>
        IUser User { get; }

        /// <summary>
        ///     <para>The parameters of the (sub) command.</para>
        ///     <para>
        ///         <b>This property will never return null.</b>
        ///     </para>
        /// </summary>
        ICommandParameters Parameters { get; }

        /// <summary>
        ///     <para>The dependency container of the context.</para>
        ///     <para>
        ///         <b>This property will never return null.</b>
        ///     </para>
        /// </summary>
        IDependencyContainer Container { get; }

        /// <summary>
        ///     Sends the command usage to the user.
        /// </summary>
        Task SendCommandUsageAsync();
    }
}