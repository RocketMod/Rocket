using Rocket.API.Eventing;
using Rocket.API.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.Core.Eventing
{
    public class EventManager : IEventManager
    {
        public void RegisterAllEvents(IPlugin plugin)
        {
            throw new NotImplementedException();
        }

        public void RegisterEvents(IEventListener listener, IPlugin plugin)
        {
            throw new NotImplementedException();
        }

        public void TriggerEvent(Event @event)
        {
            throw new NotImplementedException();
        }

        public void UnregisterAllEvents(IPlugin plugin)
        {
            throw new NotImplementedException();
        }

        public void UnregisterEvents(IEventListener listener, IPlugin plugin)
        {
            throw new NotImplementedException();
        }
    }
}
