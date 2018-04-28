using System.Collections.Generic;

namespace Rocket.API.Eventing
{
    /// <summary>
    ///     Base representation of an event.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        ///     The name of the event.
        /// </summary>
        string Name { get; }

        /// <summary>
        ///     Defines if the event is broadcasted globally to all listeners.
        /// </summary>
        bool IsGlobal { get; }

        /// <summary>
        ///     Defines how and when the event should be fired.
        /// </summary>
        EventExecutionTargetContext ExecutionTarget { get; }

        /// <summary>
        ///     The arguments of the event.
        /// </summary>
        Dictionary<string, object> Arguments { get; }
    }
}