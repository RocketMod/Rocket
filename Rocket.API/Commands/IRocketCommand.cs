using System.Collections.Generic;
using Rocket.API.Player;
using UnityEngine;

namespace Rocket.API.Commands
{
    public enum AllowedCaller { Console, Player, Both }

    public interface IRocketCommand
    {
        AllowedCaller AllowedCaller { get; }
        string Name { get; }
        string Help { get; }
        string Syntax { get; }
        List<string> Aliases { get; }
        List<string> Permissions { get; }
        void Execute(ICommandContext ctx);
    }

    public interface ICommandContext
    {
        IRocketPlayer Caller { get; }
        string[] Arguments { get; }
        void Print(string msg);
        void Print(string msg, Color color);
    }
}