using Rocket.API.Commands;
using Rocket.API.Eventing;
using Rocket.API.Player;

namespace Rocket.Core.Player.Events
{
    public class PlayerUnbanEvent: Event, ICancellableEvent
    {
        public PlayerUnbanEvent(IPlayer player, IUser caller = null) : base(true)
        {
            Player = player;
            Caller = caller;
        }

        public PlayerUnbanEvent(IPlayer player, IUser caller = null, bool global = true) : base(global)
        {
            Player = player;
            Caller = caller;
        }

        public PlayerUnbanEvent(IPlayer player, IUser caller = null,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(executionTarget, global)
        {
            Player = player;
            Caller = caller;
        }

        public PlayerUnbanEvent(IPlayer player, IUser caller = null, string name = null,
                              EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                              bool global = true) : base(name, executionTarget, global)
        {
            Player = player;
            Caller = caller;
        }

        public IPlayer Player { get; }
        public IUser Caller { get; }

        public bool IsCancelled { get; set; }

    }
}