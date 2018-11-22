using System;

namespace Rocket.Core.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandUserAttribute : Attribute
    {
        public CommandUserAttribute(Type type)
        {
            Type = type;
        }

        public Type Type { get; }
    }
}