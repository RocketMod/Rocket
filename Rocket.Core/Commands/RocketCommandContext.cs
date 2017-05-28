using Rocket.API.Commands;
using Rocket.API.Player;
using UnityEngine;

namespace Rocket.Core.Commands
{
    public class RocketCommandContext: ICommandContext
    {
        public RocketCommandContext(IRocketPlayer caller, string[] arguments, IRocketCommand command)
        {
            Caller = caller;
            Parameters = arguments;
            Command = command;
        }

        public IRocketPlayer Caller { get; }
        public string[] Parameters { get; }
        public IRocketCommand Command { get; }

        public void Print(string msg)
        {
            R.Implementation.Chat.Say(Caller, msg);
        }

        public void Print(string msg, Color color)
        {
            R.Implementation.Chat.Say(Caller, msg, color);
        }
    }
}