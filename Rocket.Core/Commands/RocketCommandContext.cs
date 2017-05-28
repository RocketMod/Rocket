using Rocket.API.Commands;
using Rocket.API.Player;
using UnityEngine;

namespace Rocket.Core.Commands
{
    public class RocketCommandContext: ICommandContext
    {
        public RocketCommandContext(IRocketPlayer caller, string[] arguments)
        {
            Caller = caller;
            Arguments = arguments;
        }

        public IRocketPlayer Caller { get; }
        public string[] Arguments { get; }
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