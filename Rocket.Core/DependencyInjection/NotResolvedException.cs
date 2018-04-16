using System;

namespace Rocket.Core.DependencyInjection
{
    public sealed class NotResolvedException : Exception
    {
        public readonly string Mapping;
        public readonly Type RequestedInterface;

        public NotResolvedException(Type @interface, string mapping)
            : base(string.Format("No implementation was found for {0} under the {1} mapping.", @interface.Name,
                mapping == null ? "default" : mapping))
        {
            RequestedInterface = @interface;
            Mapping = mapping;
        }

        public NotResolvedException(Type @interface)
            : base(string.Format("No implementations were found for {0}.", @interface.Name))
        {
            RequestedInterface = @interface;
            Mapping = null;
        }
    }
}