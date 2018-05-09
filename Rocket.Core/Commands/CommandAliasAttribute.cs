using System;

namespace Rocket.Core.Commands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandAliasAttribute : Attribute
    {
        public CommandAliasAttribute(string aliasName)
        {
            AliasName = aliasName;
        }

        public string AliasName { get; }
    }
}