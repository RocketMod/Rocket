using System.Collections.Generic;
using Rocket.API.Eventing;

namespace Rocket.Core.Eventing
{
    public class GenericEvent : Event
    {
        public GenericEvent(string name, EventExecutionTargetContext ctx = EventExecutionTargetContext.Sync) :
            base(name, ctx)
        {
            Arguments = new Dictionary<string, object>();
        }

        public Dictionary<string, object> Arguments { get; }
    }
}