using Rocket.API.User;
using System;

namespace Rocket.Core.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandUserAttribute : Attribute
    {
        public CommandUserAttribute(UserType type)
        {
            Type = type;
        }

        public UserType Type { get; }
    }
}