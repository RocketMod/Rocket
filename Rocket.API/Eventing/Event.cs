using System.Collections.Generic;
using System.Reflection;

namespace Rocket.API.Eventing
{
    public class Event : IEvent
    {
        protected Event() : this(null)
        {

        }

        /// <param name="executionTarget">When and where should the event be called? See <see cref="ExecutionTarget"/></param>
        protected Event(EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync) : this(null, executionTarget)
        {
            
        }

        /// <param name="executionTarget">When and where should the event be called? See <see cref="ExecutionTarget"/></param>
        /// <param name="name">The name of the event. Will be auto set when null.</param>
        protected Event(string name = null, EventExecutionTargetContext executionTarget = EventExecutionTargetContext.Sync)
        {
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

        public bool Global { get; private set; }
    }
}
