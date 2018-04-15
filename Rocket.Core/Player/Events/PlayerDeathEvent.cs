using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Player
{
    public class PlayerDeathEvent : PlayerEvent
    {
        public IPlayer Killer { get; }

        public PlayerDeathEvent(IPlayer player, IPlayer killer = null) : base(player)
        {
            Killer = killer;
        }

        public PlayerDeathEvent(IPlayer player, IPlayer killer = null, bool global = true) : base(player, global)
        {
            Killer = killer;
        }

        public PlayerDeathEvent(IPlayer player, IPlayer killer = null,
                                EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                bool global = true) : base(player, executionTarget, global)
        {
            Killer = killer;
        }

        public PlayerDeathEvent(IPlayer player, IPlayer killer = null, string name = null,
                                EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                bool global = true) : base(player, name, executionTarget, global)
        {
            Killer = killer;
        }
    }
}