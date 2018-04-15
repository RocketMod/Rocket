namespace Rocket.API.Commands
{
    public interface ICommand
    {
        string Name { get; }

        string Permission { get; }

        void Execute(ICommandContext context);

        bool SupportsCaller(ICommandCaller caller);
    }
}