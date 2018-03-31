using Rocket.API.Eventing;
using Rocket.API.Plugin;

namespace Rocket.Core.Eventing
{
    public static class EventExtensions 
    {
        /// <summary>
        /// Shortcut for <see cref="IEventManager"/>.TriggerEvent
        /// </summary>
        public static void Fire(this IEvent @this, IPlugin plugin)
        {
            plugin.Container.GetInstance<IEventManager>().Emit(plugin, @this);
        }
    }
}