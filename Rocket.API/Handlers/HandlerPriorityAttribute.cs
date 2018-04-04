using System;

namespace Rocket.API.Handlers {
    public class HandlerPriorityAttribute : Attribute {
        public virtual HandlerPriority Priority { get; set; }
    }
}