using System;
using Rocket.API.Commands;
using Rocket.API.Entities;
using Rocket.API.Math;

namespace Rocket.API.Player
{
    public delegate void PlayerMove(RVector3 oldPos, RVector3 newPos);
    public interface IOnlinePlayer : IPlayer, ICommandCaller, IEntity
    {
        DateTime SessionConnectTime { get; }

        DateTime? SessionDisconnectTime { get; }
        
        TimeSpan SessionOnlineTime { get; }

        //Not sure if this event should be a normal event because of the high frequency it gets called
        event PlayerMove OnPlayerMove;
    }
}