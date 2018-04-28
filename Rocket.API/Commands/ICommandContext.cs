using Rocket.API.DependencyInjection;

namespace Rocket.API.Commands
{
    /// <summary>
    /// A command execution context. Includes various information about the environment the command was called in.
    /// </summary>
    public interface ICommandContext
    {
        /// <summary>
        /// If the command is a 
        /// </summary>
        ICommandContext ParentCommandContext { get; }

        /// <summary>
        /// The prefix used to call this command.<br/>
        /// Useful when sending command usage messages.<br/><br/>
        /// <b>Example:</b><br/>
        /// When the command was executed using "/mycommand", it will be "/", when it was executed using "!mycommand", it will be "!".<br/><br/>
        /// <see cref="ISubCommand">Sub commands</see> include their parents: <br/>
        /// "/mycommand sub" will return "/mycommand" as prefix.
        /// </summary>
        /// <remarks>
        /// </remarks>
        string CommandPrefix { get; }

        /// <summary>
        /// The alias or name used to execute this command.
        /// </summary>
        string CommandAlias { get; }

        /// <summary>
        /// The command associated with the context.
        /// <b>This property will never return null.</b>
        /// </summary>
        ICommand Command { get; }

        /// <summary>
        /// The caller of the command.<br/>
        /// Is guaranteed to be a <see cref="ICommand.SupportsCaller">supported caller</see>.<br/><br/>
        /// <b>This property will never return null.</b>
        /// </summary>
        ICommandCaller Caller { get; }

        /// <summary>
        /// The parameters of the command.
        /// <b>This property will never return null.</b>
        /// </summary>
        ICommandParameters Parameters { get; }

        /// <summary>
        /// The IoC container of the context.
        /// <b>This property will never return null.</b>
        /// </summary>
        IDependencyContainer Container { get; }
    }
}