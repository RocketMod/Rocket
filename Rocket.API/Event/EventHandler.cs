using System;

namespace Rocket.API.Event
{
    /// <summary>
    /// Marks a method as listener for a specific <see cref="Event"/> defined in the first argument of the event.
    /// <para />
    /// Important: Methods with this attribute may only have one argument, which is a class extending the <see cref="Event"/> type.
    /// <example>
    /// <code>
    /// [EventHandler]
    /// public void OnPlayerDamage(PlayerDamageEvent @event)
    /// {
    ///     ...
    /// }
    /// </code>
    /// </example>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandler : Attribute
    {
        public EventPriority Priority = EventPriority.NORMAL;
        public bool IgnoreCancelled = false;
    }
}