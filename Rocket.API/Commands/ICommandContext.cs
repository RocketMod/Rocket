using Rocket.API.DependencyInjection;

namespace Rocket.API.Commands
{
    /// <summary>
    ///     A command execution context. Includes various information about the environment the command was called in.
    /// </summary>
    public interface ICommandContext
    {
        /// <summary>
        ///     The parent command context for sub commands.
        ///     <para><b>This property can return null.</b></para>
        /// </summary>
        /// <example>
        ///     If the command was entered as "/mycommand sub", this will return the parent context with parameters "sub".
        /// </example>
        ICommandContext ParentCommandContext { get; }

        /// <summary>
        ///     The root command context.
        ///     <para><b>This property will never return null.</b></para>
        /// </summary>
        ICommandContext RootCommandContext { get; }

        /// <summary>
        ///     <para>The prefix used to call this (sub) command.</para>
        ///     <para>Useful when sending command usage messages.</para>
        ///     <para>
        ///         <see cref="ISubCommand">Sub commands</see> include their parents.
        ///     </para>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <example>
        ///     <para>
        ///         If the command was executed using "/mycommand", it will be "/", when it was executed using "!mycommand", it will be "!". 
        ///     </para>
        ///     <para>
        ///         If the command was a subcommand "sub", "/mycommand sub" will return "/mycommand" as prefix.
        ///     </para>
        /// </example>
        string CommandPrefix { get; }

        /// <summary>
        ///     The alias or name used to execute this (sub) command.
        /// </summary>
        string CommandAlias { get; }

        /// <summary>
        ///     <para>The (sub) command associated with the context.</para>
        ///     <para><b>This property will never return null.</b></para>
        /// </summary>
        ICommand Command { get; }

        /// <summary>
        ///     <para>The caller of the command.</para>
        ///     <para>Is guaranteed to be a <see cref="ICommand.SupportsCaller">supported command caller</see>.</para>
        ///     <para><b>This property will never return null.</b></para>
        /// </summary>
        ICommandCaller Caller { get; }

        /// <summary>
        ///     <para>The parameters of the (sub) command.</para>
        ///     <para><b>This property will never return null.</b></para>
        /// </summary>
        ICommandParameters Parameters { get; }

        /// <summary>
        ///     <para>The dependency container of the context.</para>
        ///     <para><b>This property will never return null.</b></para>
        /// </summary>
        IDependencyContainer Container { get; }
    }
}