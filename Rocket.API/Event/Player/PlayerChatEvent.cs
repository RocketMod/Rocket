using Rocket.API.Player;
using UnityEngine;

namespace Rocket.API.Event.Player
{
    public class PlayerChatEvent : PlayerEvent, ICancellableEvent
    {
        public Color Color { get; }
        public string Message { get; }
        public bool IsCancelled { get; set; }
        public PlayerChatMode ChatMode { get; set; }

        public PlayerChatEvent(IRocketPlayer player, Color color, string message, PlayerChatMode chatmode) : base(player)
        {
            Color = color;
            Message = message;
            ChatMode = chatmode;
        }
    }

    public enum PlayerChatMode
    {
        Global,
        Local,
        Group,
        Welcome,
        Say
    }
}