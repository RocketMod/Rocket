using System;
using Rocket.API.Commands;

namespace Rocket.Core.Commands
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Permission { get; set; }

        public string Syntax { get; set; }

        public string[] Aliases { get; set; }

        public Type[] SupportedCommandCallers = { typeof(ICommandCaller)};
    }
}