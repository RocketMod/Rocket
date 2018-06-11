using System;

namespace Rocket.Core.Configuration
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class CommentAttribute: Attribute
    {
        public string Comment { get; }

        public CommentAttribute(string comment)
        {
            Comment = comment;
        }
    }
}