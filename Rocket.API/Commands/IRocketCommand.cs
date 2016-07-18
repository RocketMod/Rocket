using Rocket.API.Plugins;
using System.Collections.Generic;

namespace Rocket.API.Commands
{
    public enum AllowedCaller { Console, Player, Both }

    public interface IRocketCommand<T> where T : IRocketPlugin
    {
        IRocketPluginManager<T> Manager { get; }
        AllowedCaller AllowedCaller { get; }
        string Name { get; }
        string Identifier { get; }
        string Help { get; }
        string Syntax { get; }
        List<string> Aliases { get; }
        List<string> Permissions { get; }

        void Execute(IRocketPlayer caller, string[] command);
    }
}