using System;
using Rocket.API.Player;
using Rocket.API.Providers.Logging;
using UnityEngine;

namespace Rocket.Core.Player
{
    public class ConsolePlayer : IRocketPlayer
    {
        public string Id => "Console";

        public string DisplayName => "Console";

        public bool IsAdmin => true;

        public int CompareTo(object obj)
        {
            return Id.CompareTo(obj);
        }

        public void Kick(string message)
        {
            //
        }

        public void Ban(string message, uint duration = 0)
        {
            //
        }

        public bool HasPermission(string permission)
        {
            return true;
        }

        public void Message(string message, Color? color)
        {
            R.Logger.LogMessage(LogLevel.INFO, message, ConsoleColor.Gray);
        }
    }
}