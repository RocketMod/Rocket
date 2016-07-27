using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Rocket.API.Chat
{
    public delegate void PlayerChatted(IRocketPlayer player, ref Color color, string message,ref bool cancel);
    public interface IChat
    {
        event PlayerChatted OnPlayerChatted;
        void Say(IRocketPlayer player, string message, Color? color = null);
        void Say(string message, Color? color = null);
    }
}
