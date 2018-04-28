using Rocket.API.Commands;
using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Player.Events
{
    public class PlayerDamageEvent : OnlinePlayerEvent, ICancellableEvent
    {
        public PlayerDamageEvent(IOnlinePlayer player, double damage, ICommandCaller damageDealer) : base(player)
        {
            Damage = damage;
            DamageDealer = damageDealer;
        }

        public PlayerDamageEvent(IOnlinePlayer player, double damage, ICommandCaller damageDealer, bool global = true) :
            base(player, global)
        {
            Damage = damage;

            DamageDealer = damageDealer;
        }

        public PlayerDamageEvent(IOnlinePlayer player, double damage, ICommandCaller damageDealer,
                                 EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                 bool global = true) : base(player, executionTarget, global)
        {
            Damage = damage;
            DamageDealer = damageDealer;
        }

        public PlayerDamageEvent(IOnlinePlayer player, double damage, ICommandCaller damageDealer, string name = null,
                                 EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                                 bool global = true) : base(player, name, executionTarget, global)
        {
            Damage = damage;
            DamageDealer = damageDealer;
        }

        public ICommandCaller DamageDealer { get; }
        public double Damage { get; set; }

        public bool IsCancelled { get; set; }
    }
}