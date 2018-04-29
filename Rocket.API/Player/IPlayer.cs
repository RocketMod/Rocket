using System;
using Rocket.API.Permissions;

namespace Rocket.API.Player
{
    public interface IPlayer : IIdentifiable, IFormattable
    {
        bool IsOnline { get; }

        DateTime? LastSeen { get; }
    }
}