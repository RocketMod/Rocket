using System;

namespace Rocket.Core.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class UserAttribute : Attribute
    {
        public UserAttribute(Type supportedCaller)
        {
            SupportedCaller = supportedCaller;
        }

        public Type SupportedCaller { get; }
    }
}