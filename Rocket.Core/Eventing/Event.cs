using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Rocket.API.Eventing;
using Rocket.Core.Extensions;

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

            List<string> names = new List<string>();
            foreach (var type in GetType().GetTypeHierarchy())
            {
                if (!typeof(IEvent).IsAssignableFrom(type))
                    break;

                if (type == typeof(Event))
                    break;

                var attr = type.GetCustomAttributes(typeof(EventNameAttribute), false)
                               .Cast<EventNameAttribute>()
                               .ToList();
                if (attr.Count == 0)
                {
                    names.Add(type.Name.Replace("Event", ""));
                    continue;
                }

                names.AddRange(attr.Select(c => c.EventName));
            }

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
            foreach (var type in GetType().GetTypeHierarchy())
            {
                if (type == GetType())
                    continue;

                if (!typeof(IEvent).IsAssignableFrom(type))
                    break;

                if (type == typeof(Event))
                    break;

                var attr = type.GetCustomAttributes(typeof(EventNameAttribute), false)
                               .Cast<EventNameAttribute>()
                               .ToList();
                if (attr.Count == 0)
                {
                    names.Add(type.Name.Replace("Event", ""));
                    continue;
                }

                names.AddRange(attr.Select(c => c.EventName));
            }

            Names = names;

            ExecutionTarget = executionTarget;
        }

        /// <summary>
        ///     <inheritdoc /><br /><br />
        ///     In this implementation it contains the properties of the class with their respective values.
        /// </summary>
        public Dictionary<string, object> Arguments
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