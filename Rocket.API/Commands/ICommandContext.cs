using Rocket.API.Player;
using UnityEngine;

namespace Rocket.API.Commands
{
    public interface ICommandContext
    {
        IRocketPlayer Caller { get; }
        IRocketCommand Command { get; }
        string[] Parameters { get; }
        void Print(string msg);
        void Print(string msg, Color color);
    }
}