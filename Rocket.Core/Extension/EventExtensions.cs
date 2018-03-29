using System;
using Rocket.API.Eventing;

namespace Rocket.Extension
{
    public static class EventExtensions 
    {
        /// <summary>
        /// Shortcut for <see cref="IEventManager"/>.TriggerEvent
        /// </summary>
        public static void Fire(this Event @this)
        {
            var manager = R.ServiceLocator.GetInstance<IEventManager>();
            if (manager == null) throw new Exception("EventManager instance is null");
            manager.TriggerEvent(@this);
        }
    }
}