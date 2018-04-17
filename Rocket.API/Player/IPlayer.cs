using System;
using Rocket.API.Commands;
using Rocket.API.Permissions;

namespace Rocket.API.Player
{
    public interface IPlayer : ICommandCaller, IFormattable, IPermissible
    {
        bool IsOnline { get; }
    }
}