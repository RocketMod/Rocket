using System;
using Rocket.API;
using Rocket.API.Eventing;
using Rocket.API.Handlers;

namespace Rocket.Core.Eventing
{
    [AttributeUsage(AttributeTargets.Method)]
    public class EventHandler : HandlerPriorityAttribute
    {
        public override HandlerPriority Priority { get; set; } = HandlerPriority.Normal;

        public bool IgnoreCancelled { get; set; } = false;

        public string EmitterName { get; set; }
    }
}