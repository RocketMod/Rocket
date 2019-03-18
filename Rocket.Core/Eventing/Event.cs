using System;
using System.Collections.Generic;
using System.Reflection;
using Rocket.API.Eventing;

namespace Rocket.Core.Eventing
{
    /// <inheritdoc />
    public class Event : IEvent
    {
        /// <param name="global">Defines if the event is emitted globally</param>
        protected Event(bool global = true) : this(EventExecutionTargetContext.Sync, global) { }

        /// <param name="executionTarget">When and where should the event be called? See <see cref="ExecutionTarget" /></param>
        /// <param name="global">Defines if the event is emitted globally</param>
        protected Event(EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                        bool global = true)
        {
            IsGlobal = global;

            List<string> names = EventBus.GetEventNames(GetType());

            Names = names;

            ExecutionTarget = executionTarget;
        }

        //child events whose parents are using this constructor will not notify parent event listeners
        [Obsolete("Use [EventName(\"name\")] in combinition with the other constructors instead.")]
        protected Event(string name, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync,
                        bool global = true)
        {
            IsGlobal = global;

            List<string> names = new List<string> { name };
            names.AddRange(EventBus.GetEventNames(GetType().BaseType));

            Names = names;

            ExecutionTarget = executionTarget;
        }

        /// <summary>
        ///     <inheritdoc /><br /><br />
        ///     In this implementation it contains the properties of the class with their respective values.
        /// </summary>
        public virtual Dictionary<string, object> Arguments
        {
            get
            {
                Dictionary<string, object> args = new Dictionary<string, object>();
                PropertyInfo[] props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (PropertyInfo prop in props)
                {
                    MethodInfo getter = prop.GetGetMethod(false);
                    if (getter == null) continue;

                    args.Add(prop.Name.ToLower(), this);
                }

                return args;
            }
        }

        /// <inheritdoc />
        public IEnumerable<string> Names { get; }

        /// <inheritdoc />
        public EventExecutionTargetContext ExecutionTarget { get; }

        /// <inheritdoc />
        public bool IsGlobal { get; }
    }
}