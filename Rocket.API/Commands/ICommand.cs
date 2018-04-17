using System.Collections.Generic;

namespace Rocket.API.Commands
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        string Permission { get; }

        List<ICommand> ChildCommands { get; }

        string GetSyntax(ICommandContext context);
        string GetHelpText(ICommandContext context);

        List<string> Aliases { get; }

        bool SupportsCaller(ICommandCaller caller);

        void Execute(ICommandContext context);

    }
}