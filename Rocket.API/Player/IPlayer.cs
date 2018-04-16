using System;
using System.ComponentModel;
using Rocket.API.Commands;

namespace Rocket.API.Player
{
    public interface IPlayer : ICommandCaller, IFormattable
    {
        double Health { get; set; }
        double MaxHealth { get; set; }
    }
}