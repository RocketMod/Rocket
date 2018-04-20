using System.Collections.Generic;

namespace Rocket.API.Commands
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        string Permission { get; }
        string Syntax { get; }

        ISubCommand[] ChildCommands { get; }
        string[] Aliases { get; }

        bool SupportsCaller(ICommandCaller caller);

        void Execute(ICommandContext context);
    }
}