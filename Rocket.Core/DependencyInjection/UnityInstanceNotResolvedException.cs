using System;

namespace Rocket.API.DependencyInjection {
    public sealed class UnityInstanceNotResolvedException : Exception {
        public readonly string Mapping;
        public readonly Type RequestedInterface;

        public UnityInstanceNotResolvedException(Type @interface, string mapping)
            : base(string.Format("No implementation was found for {0} under the {1} mapping.", @interface.Name,
                mapping == null ? "default" : mapping)) {
            RequestedInterface = @interface;
            Mapping = mapping;
        }

        public UnityInstanceNotResolvedException(Type @interface)
            : base(string.Format("No implementations were found for {0}.", @interface.Name)) {
            RequestedInterface = @interface;
            Mapping = null;
        }
    }
}