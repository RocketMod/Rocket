using System.Collections.Generic;

namespace Rocket.API.Eventing
{
    public interface IEventListener
    {
        IDictionary<IEvent,EventCallback> Callbacks { get; }
    }
}