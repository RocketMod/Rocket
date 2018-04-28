namespace Rocket.API.Commands
{
    /// <summary>
    /// A sub command. See <see cref="ICommand.ChildCommands"/>.
    /// </summary>
    public interface ISubCommand : ICommand
    {

    }

    /// <summary>
    /// <inheritdoc cref="ISubCommand"/>
    /// </summary>
    /// <typeparam name="TParentCommand">The parent command.</typeparam>
    public interface ISubCommand<TParentCommand> : ISubCommand where TParentCommand : ICommand
    {

    }
}