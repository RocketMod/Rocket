using System;
using Rocket.API.Player;
using UnityEngine;

namespace Rocket.Core.Player
{
    public class ConsolePlayer : IPlayer
    {
        public string Id => "Console";

        public string DisplayName => "Console";

        public bool IsAdmin => true;

        public int CompareTo(object obj)
        {
            return Id.CompareTo(obj);
        }
    }
}