using System;

namespace Rocket.Core.Configuration
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class SetupAttribute : Attribute
    {
        public bool IsOptional { get; set; }
    }
}