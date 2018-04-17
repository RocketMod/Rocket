using Rocket.API.DependencyInjection;

namespace Rocket.API.Commands
{
    public interface ICommandContext
    {
        ICommandContext ParentCommandContext { get; }

        string CommandPrefix { get; }

        string CommandAlias { get; }

        ICommand Command { get; }

        ICommandCaller Caller { get; }

        ICommandParameters Parameters { get; }

        IDependencyContainer Container { get; }
    }
}