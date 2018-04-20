using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Player.Events
{
    public class PlayerPreConnectEvent : Event, ICancellableEvent
    {
        public IPlayer Player { get; }

        public virtual string RejectionReason { get; set; }

        public PlayerPreConnectEvent(IPlayer player) : base(true)
        {
            Player = player;
        }
        public PlayerPreConnectEvent(IPlayer player, bool global = true) : base(global)
        {
            Player = player;
        }

        public PlayerPreConnectEvent(IPlayer player,
                                     EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                     bool global = true) : base(executionTarget, global)
        {
            Player = player;
        }

        public PlayerPreConnectEvent(IPlayer player, string name = null,
                                     EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                     bool global = true) : base(name, executionTarget, global)
        {
            Player = player;
        }

        public bool IsCancelled { get; set; }
    }
}