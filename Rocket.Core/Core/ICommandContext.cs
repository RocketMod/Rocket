using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Core
{
    public interface ICommandContext
    {
        IPlayer Caller { get; }
        ICommand Command { get; }
        string[] Parameters { get; }
    }
}
