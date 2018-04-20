using Rocket.API.Entities;
using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Player.Events
{
    public class PlayerDeathEvent : PlayerEvent
    {
        public IEntity Killer { get; }

        public PlayerDeathEvent(IOnlinePlayer player, IEntity killer = null) : base(player)
        {
            Killer = killer;
        }

        public PlayerDeathEvent(IOnlinePlayer player, IEntity killer = null, bool global = true) : base(player, global)
        {
            Killer = killer;
        }

        public PlayerDeathEvent(IOnlinePlayer player, IEntity killer = null,
                                EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                bool global = true) : base(player, executionTarget, global)
        {
            Killer = killer;
        }

        public PlayerDeathEvent(IOnlinePlayer player, IEntity killer = null, string name = null,
                                EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                bool global = true) : base(player, name, executionTarget, global)
        {
            Killer = killer;
        }
    }
}