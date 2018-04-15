using System;

namespace Rocket.Core.ServiceProxies
{
    public class ServicePriorityAttribute : Attribute
    {
        public virtual ServicePriority Priority { get; set; }
    }
}