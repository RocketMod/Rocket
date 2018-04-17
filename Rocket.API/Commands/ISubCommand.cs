namespace Rocket.API.Commands
{
    public interface ISubCommand : ICommand
    {

    }

    public interface ISubCommand<TParentCommand> : ISubCommand where TParentCommand : ICommand
    {

    }
}