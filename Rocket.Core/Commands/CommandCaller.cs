using System;

namespace Rocket.Core.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandUserAttribute : Attribute
    {
        public CommandUserAttribute(Type userType)
        {
            UserType = userType;
        }

        public Type UserType { get; }
    }
}