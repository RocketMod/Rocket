namespace Rocket.API.Commands
{
    public interface ICommand
    {
        string[] Permissions { get; }
        
        void Execute(ICommandContext context);
    }
}
