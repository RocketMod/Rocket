using Rocket.API.DependencyInjection;

namespace Rocket.API.Commands
{
    public interface ICommandContext
    {
        string CommandPrefix { get; }

        string CommandAlias { get; }

        ICommand Command { get; }

        ICommandCaller Caller { get; }

        ICommandParameters Parameters { get; }

        IDependencyContainer Container { get; }
    }
}