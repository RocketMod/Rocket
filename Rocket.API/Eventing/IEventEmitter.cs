using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.API.Eventing
{
    public interface IEventEmitter
    {
        string Name { get; }
    }
}
