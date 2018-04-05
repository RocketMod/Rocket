namespace Rocket.API.Commands
{
    public interface ICommand
    {
        string Name { get; }

        string[] Permissions { get; }

        void Execute(ICommandContext context);
    }
}