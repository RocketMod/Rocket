using System.Collections.Generic;

namespace Rocket.API.Commands
{
    public interface ICommand
    {
        List<string> Aliases { get; }

        string Name { get; }

        string Permission { get; }

        void Execute(ICommandContext context);

        bool SupportsCaller(ICommandCaller caller);
    }
}