using System;
using Rocket.API.Commands;
using Rocket.API.Entities;
using Rocket.API.Math;

namespace Rocket.API.Player
{
    public interface IOnlinePlayer : IPlayer, ICommandCaller, IEntity
    {
        DateTime SessionConnectTime { get; }

        DateTime? SessionDisconnectTime { get; }
        
        TimeSpan SessionOnlineTime { get; }
    }
}