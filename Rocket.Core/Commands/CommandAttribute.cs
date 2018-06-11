using System;

namespace Rocket.Core.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandAttribute : Attribute
    {
        public string Name { get; set; }

        public string Summary { get; set; }

        public string Syntax { get; set; }

        public string Description { get; set; }
    }
}