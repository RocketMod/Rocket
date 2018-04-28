using System;
using Rocket.API.Permissions;

namespace Rocket.API.Player
{
    public interface IPlayer : IPermissible
    {
        bool IsOnline { get; }

        DateTime? LastSeen { get; }
    }
}