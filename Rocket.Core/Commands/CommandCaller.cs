using System;

namespace Rocket.Core.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandCallerAttribute :Attribute
    {
        public Type SupportedCaller { get; }

        public CommandCallerAttribute(Type supportedCaller)
        {
            SupportedCaller = supportedCaller;
        }
    }
}