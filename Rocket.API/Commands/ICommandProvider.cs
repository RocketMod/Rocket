using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.API.Commands
{
    public interface ICommandProvider
    {
        IEnumerable<ICommand> Commands { get; }
    }
}
