using System;
using System.Threading.Tasks;
using Rocket.API.User;

namespace Rocket.API.Commands
{
    /// <summary>
    ///     Base interface for commands.
    ///     <para>
    ///         Commands are usually executed ingame by players or by the console. They are
    ///         a human interface to plugin functionalities which allow to do specific actions.
    ///     </para>
    ///     <para>
    ///         <b>Example commands include: </b>/help, !tp, ~heal, etc. (the prefixes like "/", "!", "~" are up to the
    ///         <see cref="ICommandHandler">command handler</see>).
    ///     </para>
    ///     <para>
    ///         In the default <see cref="ICommandProvider">command provider</see> implementation, non abstract classes
    ///         inheriting this interface are automatically found
    ///         and registered.
    ///     </para>
    ///     <para>
    ///         Usually commands should not be called by plugins programatically.
    ///     </para>
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        ///     <para>The primary name of the command, which will be used to execute it.</para>
        ///     <para>The primary name overrides any <see cref="Aliases">aliases</see> of other commands by default.</para>
        ///     <para>
        ///         <b>This property must never return null.</b>
        ///     </para>
        /// </summary>
        /// <example>
        ///     If the name is "Help", the command will be usually be called using "/heal" (or just "heal" in console)
        /// </example>
        string Name { get; }

        /// <summary>
        ///     <para>The aliases of the command, which are often shorter versions of the primary name.</para>
        ///     <para>
        ///         <b>This property can return null.</b>
        ///     </para>
        /// </summary>
        /// <example>
        ///     If the aliases are "h" and "he", the command will be callable using "/h" or "/he".
        /// </example>
        string[] Aliases { get; }

        /// <summary>
        ///     The command summary.
        ///     <para><b>This proprty must not return null.</b>.</para>
        /// </summary>
        /// <example>
        ///     <c>"This command heals you or someone else."</c>
        /// </example>
        string Summary { get; }

        /// <summary>
        ///     The full description of the command.
        ///     <para>
        ///         <b>This proprty can return null</b>.
        ///     </para>
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     The command syntax will be shown to the <see cref="IUser" /> when the command was not used correctly.
        ///     <para>An output for the above example could be "/heal [player] &lt;amount&gt;".</para>
        ///     <para>The syntax should not contain Child Command usage.</para>
        ///     <para>
        ///         <b>This property must never return null.</b>
        ///     </para>
        /// </summary>
        /// <remarks>
        ///     [...] means optional argument and &lt;...&gt; means required argument, so in this case "player" is an optional
        ///     argument while "amount" is a required one.
        /// </remarks>
        /// <example>
        ///     <c>"[player] &lt;amount&gt;"</c>
        /// </example>
        string Syntax { get; }

        /// <summary>
        ///     The child commands of this command. Child commands override the <see cref="ExecuteAsync" /> method in some cases (see
        ///     examples).
        ///     <para>
        ///         <b>This property can return null.</b>
        ///     </para>
        ///     <seealso cref="IChildCommand" />
        /// </summary>
        /// <example>
        ///     Assume a command was entered as "/mycommand sub 1 2". If the <see cref="Name">name</see> or an
        ///     <see cref="Aliases">alias</see> of this command equals "mycommand" and a child command with the
        ///     <see cref="Name">name</see>
        ///     or an <see cref="Aliases">alias</see> "sub" exists, then the <see cref="ExecuteAsync">Execute</see> method of the child
        ///     command will be called with "1" and "2" as parameters, while the <see cref="ExecuteAsync">Execute</see> method of the
        ///     parent will not be called.
        /// </example>
        IChildCommand[] ChildCommands { get; }

        /// <summary>
        ///     Defines if this command can be executed by the given user type.
        ///     It is guaranteed that <see cref="ExecuteAsync" /> can only be called by supported users.
        /// </summary>
        /// <param name="user">The user type to check.</param>
        /// <returns><b>true</b> if the given user type can execute this command; otherwise, <b>false</b>.</returns>
        bool SupportsUser(IUser user);

        /// <summary>
        ///     Executes the command if no Child Command is involved.
        /// </summary>
        /// <example>
        ///     Assume a command was entered as "/mycommand" (also assuming the command prefix is "/"). If the
        ///     <see cref="Name">name</see> or
        ///     an <see cref="Aliases">alias</see> of this command equals "mycommand", then this method will be executed.
        /// </example>
        /// <seealso cref="ChildCommands" />
        /// <param name="context">The <see cref="ICommandContext">context</see> of the command.</param>
        Task ExecuteAsync(ICommandContext context);
    }
}