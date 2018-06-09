using System.Collections.Generic;

namespace Rocket.API.Eventing
{
    /// <summary>
    ///     Base representation of an event.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        ///     The name of the event with the name of the parent types.
        ///     For example: "playerconnected", "userconnected".
        ///     <b>Each event instance should only have one name.</b>
        /// </summary>
        IEnumerable<string> Names { get; }

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