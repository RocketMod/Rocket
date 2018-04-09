using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Events.Player
{
    public class PlayerDeathEvent : PlayerEvent
    {
        public string DeathReason { get; }

        public PlayerDeathEvent(IPlayer player, string deathReason) : base(player)
        {
            DeathReason = deathReason;
        }

        public PlayerDeathEvent(IPlayer player, string deathReason, bool global = true) : base(player, global)
        {
            DeathReason = deathReason;
        }

        public PlayerDeathEvent(IPlayer player, string deathReason,
                                EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                bool global = true) : base(player, executionTarget, global)
        {
            DeathReason = deathReason;
        }

        public PlayerDeathEvent(IPlayer player, string deathReason, string name = null,
                                EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                bool global = true) : base(player, name, executionTarget, global)
        {
            DeathReason = deathReason;
        }
    }
}