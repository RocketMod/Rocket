using System;
using System.Collections.Generic;

namespace Rocket.API.Eventing
{
    public interface IEventListener
    {
        IDictionary<Type,EventCallback> Callbacks { get; }
    }
}