using System;
using Rocket.API.Eventing;

namespace Rocket.Core.Extensibility
{
    public static class EventExtensions 
    {
        /// <summary>
        /// Shortcut for <see cref="IEventManager"/>.TriggerEvent
        /// </summary>
        public static void Fire(this Event @this)
        {
            @this.EventManager.TriggerEvent(@this);
        }
    }
}