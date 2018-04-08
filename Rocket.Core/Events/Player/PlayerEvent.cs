using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Events.Player
{
    public abstract class PlayerEvent : Event
    {
        public IPlayer Player { get; }

        /// <param name="global">Defines if the event is emitted globally</param>
        protected PlayerEvent(IPlayer player, bool global = true) : base(global)
        {
            Player = player;
        }

        /// <param name="executionTarget">When and where should the event be called? See <see cref="ExecutionTarget" /></param>
        /// <param name="global">Defines if the event is emitted globally</param>
        protected PlayerEvent(IPlayer player, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                        bool global = true) : base(executionTarget, global)
        {
            Player = player;
        }

        /// <param name="executionTarget">When and where should the event be called? See <see cref="ExecutionTarget" /></param>
        /// <param name="name">The name of the event. Will be auto set when null.</param>
        /// ///
        /// <param name="global">Defines if the event is emitted globally</param>
        protected PlayerEvent(IPlayer player, string name = null,
                        EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                        bool global = true) : base(name, executionTarget, global)
        {
            Player = player;
        }
    }
}