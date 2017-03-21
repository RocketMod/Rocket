using Rocket.API.Player;
using UnityEngine;

namespace Rocket.API.Event.Player
{
    public class PlayerChatEvent : PlayerEvent, ICancellableEvent
    {
        public Color Color { get; }
        public string Message { get; }
        public bool IsCancelled { get; set; }

        public PlayerChatEvent(IRocketPlayer player, Color color, string message) : base(player)
        {
            Color = color;
            Message = message;
        }
    }
}