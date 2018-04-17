using System.Collections.Generic;

namespace Rocket.API.Commands
{
    public interface ICommand
    {
        /* todo: not used yet */
        List<ICommand> ChildCommands { get; }

        string GetSyntax(ICommandContext context);

        string Description { get; }

        List<string> Aliases { get; }

        string GetHelpText(ICommandContext context);

        string Name { get; }

        string Permission { get; }

        void Execute(ICommandContext context);

        bool SupportsCaller(ICommandCaller caller);
    }
}