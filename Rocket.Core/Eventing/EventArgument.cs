using Rocket.API.Eventing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Core.Eventing
{
    public class EventArguments : IEventArguments
    {
        public object[] Arguments { get; private set; }
        public EventArguments(params object[] arguments)
        {
            Arguments = arguments;
        }
    }
}
