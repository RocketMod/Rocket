using Rocket.API.Player;
using UnityEngine;

namespace Rocket.API.Commands
{
    public interface ICommandContext
    {
        IPlayer Caller { get; }
        ICommand Command { get; }
        ICommandOutput Output { get; }
        string[] Parameters { get; }
    }
}