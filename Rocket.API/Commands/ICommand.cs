using System;

namespace Rocket.API.Commands
{
    /// <summary>
    ///     Base interface for commands.<br />
    ///     Commands are usually executed ingame by players or by the console. They are
    ///     a human interface to plugin functionalities which allow to do specific actions.
    ///     <br /><br />
    ///     <b>Example commands: </b>/help, /tp, /heal, etc...
    ///     <br /><br />
    ///     In the default implementation, these non abstract classes inheriting this interface are automatically found
    ///     and registered to the plugin.
    ///     <br /><br />
    ///     Usually commands are not called by plugins programatically.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        ///     The primary name of the command, which will be used to execute it.<br />
        ///     For example, if the name is "Help", the command will be usually be called using "/heal" (or just "heal" in console)
        ///     <br /><br />
        ///     The primary name overrides any <see cref="Aliases">aliases</see> of other commands by default.<br /><br />
        ///     <b>This property must never return null.</b>
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     The aliases of the command, which are often shorter versions of the primary name.
        ///     For example, if the aliases are "h" and "he", the command will be callable using "/h" or "/he" too.<br /><br />
        ///     <b>This property can return null.</b>
        /// </summary>
        string[] Aliases { get; }

        /// <summary>
        ///     The description of the command.<br />
        ///     Example: <code>"This command heals you or someone else."</code>
        ///     <b>This proprty can return null</b> but its advised to always include a description for the command.
        /// </summary>
        string Description { get; }

        /// <summary>
        ///     The permission required to execute the command.
        ///     Example: <code>MyPermission.Heal</code>
        ///     <b>This property can return null.</b><br /><br />
        /// </summary>
        /// <remarks>
        ///     When returning null, the default permission will be used, which depends on the <see cref="ICommandHandler" />
        ///     implementation.<br />
        ///     The default CommandHandler uses {PluginName}.{CommandName} (e.g. "MyPlugin.Heal") as permission.
        /// </remarks>
        string Permission { get; }

        /// <summary>
        ///     The command syntax will be shown to the <see cref="ICommandCaller" /> when the command was not used correctly.
        ///     <br /><br />
        ///     <b>Example usage:</b> <code>"[player] &lt;amount&gt;"</code>
        ///     An output for the above example could be "/heal [player] &lt;amount&gt;".<br /><br />
        ///     <b>This property must never return null.</b>
        /// </summary>
        /// <remarks>
        ///     [...] means optional argument and &lt;...&gt; means required argument, so in this case "player" is an optional
        ///     argument while "amount" is a required one.
        /// </remarks>
        string Syntax { get; }

        /// <summary>
        ///     The child commands of this command. <br /><br />
        ///     <b>Example:</b><br />
        ///     Assume a command was entered as "/mycommand sub". If the <see cref="Name">name</see> or an
        ///     <see cref="Aliases">alias</see> of this command equals "mycommand" and a child command with the
        ///     <see cref="Name">name</see>
        ///     or an <see cref="Aliases">alias</see> "sub" exists, then the <see cref="Execute">Execute</see> method of the child
        ///     command will be called.
        ///     The <see cref="Execute">Execute</see> method of the parent will not be called.<br /><br />
        ///     <b>This property can return null.</b><br /><br />
        ///     <seealso cref="ISubCommand" />
        /// </summary>
        ISubCommand[] ChildCommands { get; }

        /// <summary>
        ///     Defines if this command can be executed by the given command caller type.
        ///     It is guaranteed that <see cref="Execute" /> can only be called by supported command callers.
        /// </summary>
        /// <param name="commandCaller">The command caller type to check.</param>
        /// <returns><b>true</b> if the given command caller type can execute this command; otherwise, <b>false</b>.</returns>
        bool SupportsCaller(Type commandCaller);

        /// <summary>
        ///     This method is called when a command caller wants to execute this command.<br /><br />
        ///     <b>Example</b><br />
        ///     Assume a command was entered as "/mycommand". If the <see cref="Name">name</see> or
        ///     an <see cref="Aliases">alias</see> of this command equals "mycommand", then this method will be executed.
        /// </summary>
        /// <param name="context">The <see cref="ICommandContext">context</see> of the command.</param>
        void Execute(ICommandContext context);
    }
}