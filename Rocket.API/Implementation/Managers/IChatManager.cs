using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rocket.API.Player;
using UnityEngine;

namespace Rocket.API.Implementation.Managers
{
    public delegate void PlayerChatted(IRocketPlayer player, ref Color color, string message,ref bool cancel);
    public interface IChatManager
    {
        event PlayerChatted OnPlayerChatted;
        void Say(IRocketPlayer player, string message, Color? color = null);
        void Say(string message, Color? color = null);
    }
}
