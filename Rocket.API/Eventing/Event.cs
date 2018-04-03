using System.Collections.Generic;
using System.Reflection;

namespace Rocket.API.Eventing
{
    public class Event : IEvent
    {
        /// <param name="global">Defines if the event is emitted globally</param>
        protected Event(bool global = true) : this(null, EventExecutionTargetContext.Sync, global)
        {

        }

        /// <param name="executionTarget">When and where should the event be called? See <see cref="ExecutionTarget"/></param>
        /// <param name="global">Defines if the event is emitted globally</param>
        protected Event(EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true) : this(null, executionTarget, global)
        {
            
        }

        /// <param name="executionTarget">When and where should the event be called? See <see cref="ExecutionTarget"/></param>
        /// <param name="name">The name of the event. Will be auto set when null.</param>
        /// /// <param name="global">Defines if the event is emitted globally</param>
        protected Event(string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync, bool global = true)
        {
            Global = global;
            Name = name ?? GetType().Name.Replace("Event", "");
            ExecutionTarget = executionTarget;
        }

        public string Name { get; }
        public EventExecutionTargetContext ExecutionTarget { get; }

        public Dictionary<string, object> Arguments
        {
            get
            {
                Dictionary<string, object> args = new Dictionary<string, object>();
                var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var prop in props)
                {
                    var getter = prop.GetGetMethod(false);
                    if (getter == null)
                        continue;

                    args.Add(prop.Name.ToLower(), this);
                }

                return args;
            }
        }

        public bool Global { get; }
    }
}
