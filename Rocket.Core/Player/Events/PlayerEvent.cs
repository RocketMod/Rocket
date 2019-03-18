using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.Eventing;

namespace Rocket.Core.Player.Events
{
    public class PlayerEvent : Event
    {
        protected PlayerEvent(IPlayer player) : this(player, true) { }

        /// <param name="global">Defines if the event is emitted globally</param>
        protected PlayerEvent(IPlayer player, bool global = true) : base(global)
        {
            Player = player;
        }

        /// <param name="player">The player triggering this event.</param>
        /// <param name="executionTarget">When and where should the event be called? See <see cref="EventExecutionTargetContext" /></param>
        /// <param name="global">Defines if the event is emitted globally</param>
        protected PlayerEvent(IPlayer player,
                            EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                            bool global = true) : base(executionTarget, global)
        {
            Player = player;
        }

        public IPlayer Player { get; }
    }
}