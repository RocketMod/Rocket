using System;
using Rocket.Core.ServiceProxies;

namespace Rocket.Core.Eventing
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandler : ServicePriorityAttribute
    {
        public override ServicePriority Priority { get; set; } = ServicePriority.Normal;

        public bool IgnoreCancelled { get; set; } = false;

        public string EmitterName { get; set; }
    }
}