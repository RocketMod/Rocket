using System;

namespace Rocket.Core.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandCallerAttribute : Attribute
    {
        public CommandCallerAttribute(Type supportedCaller)
        {
            SupportedCaller = supportedCaller;
        }

        public Type SupportedCaller { get; }
    }
}