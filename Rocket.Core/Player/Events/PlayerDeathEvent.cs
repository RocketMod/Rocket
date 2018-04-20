using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Player.Events
{
    public class PlayerDeathEvent : PlayerEvent
    {
        public IOnlinePlayer Killer { get; }

        public PlayerDeathEvent(IOnlinePlayer player, IOnlinePlayer killer = null) : base(player)
        {
            Killer = killer;
        }

        public PlayerDeathEvent(IOnlinePlayer player, IOnlinePlayer killer = null, bool global = true) : base(player, global)
        {
            Killer = killer;
        }

        public PlayerDeathEvent(IOnlinePlayer player, IOnlinePlayer killer = null,
                                EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                bool global = true) : base(player, executionTarget, global)
        {
            Killer = killer;
        }

        public PlayerDeathEvent(IOnlinePlayer player, IOnlinePlayer killer = null, string name = null,
                                EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                bool global = true) : base(player, name, executionTarget, global)
        {
            Killer = killer;
        }
    }
}