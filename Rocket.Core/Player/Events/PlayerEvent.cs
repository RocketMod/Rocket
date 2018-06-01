using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.Core.User.Events;

namespace Rocket.Core.Player.Events
{
    public abstract class PlayerEvent : UserEvent
    {
        protected PlayerEvent(IPlayer player) : this(player, true) { }

        /// <param name="global">Defines if the event is emitted globally</param>
        protected PlayerEvent(IPlayer player, bool global = true) : base(player.GetUser(), global)
        {
            Player = player;
        }

        /// <param name="executionTarget">When and where should the event be called? See <see cref="ExecutionTarget" /></param>
        /// <param name="global">Defines if the event is emitted globally</param>
        protected PlayerEvent(IPlayer player,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(player.GetUser(), executionTarget, global)
        {
            Player = player;
        }

        /// <param name="executionTarget">When and where should the event be called? See <see cref="ExecutionTarget" /></param>
        /// <param name="name">The name of the event. Will be auto set when null.</param>
        /// ///
        /// <param name="global">Defines if the event is emitted globally</param>
        protected PlayerEvent(IPlayer player, string name = null,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(player.GetUser(), name, executionTarget, global)
        {
            Player = player;
        }

        public IPlayer Player { get; }
    }
}