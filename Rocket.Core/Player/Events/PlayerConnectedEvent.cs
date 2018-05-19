using System;
using Rocket.API.Eventing;
using Rocket.API.Player;
using Rocket.API.User;
using Rocket.Core.Player.Events;
using Rocket.Core.User.Events;

namespace Rocket.Core.Player.Events
{
    public class PlayerConnectedEvent: UserConnectedEvent
    {
        public IPlayer Player { get; }
        public PlayerConnectedEvent(IPlayer player) : base(player.User)
        {
            Player = player;
        }
        public PlayerConnectedEvent(IPlayer player, bool global = true) : base(player.User, global)
        {
            Player = player;
        }
        public PlayerConnectedEvent(IPlayer player, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player.User, executionTarget, global)
        {
            Player = player;
        }
        public PlayerConnectedEvent(IPlayer player, string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : base(player.User, name, executionTarget, global)
        {
            Player = player;
        }
    }
}