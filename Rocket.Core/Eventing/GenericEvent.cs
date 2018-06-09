using System.Collections.Generic;
using Rocket.API.Eventing;

namespace Rocket.Core.Eventing
{
    public class GenericEvent : Event
    {
        public GenericEvent(EventExecutionTargetContext ctx = EventExecutionTargetContext.Sync) :
            base(ctx)
        {
            Arguments = new Dictionary<string, object>();
        }

        public Dictionary<string, object> Arguments { get; set; }
    }
}