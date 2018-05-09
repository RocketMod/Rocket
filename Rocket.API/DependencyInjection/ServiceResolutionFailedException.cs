using System;

namespace Rocket.API.DependencyInjection
{
    /// <summary>
    ///     Thrown when a service could not be resolved.
    /// </summary>
    public class ServiceResolutionFailedException : Exception
    {
        /// <param name="service">The service which could not be resolved.</param>
        public ServiceResolutionFailedException(Type service) : base(
            $"Service \"{service.FullName}\" could not be resolved.")
        {
            Service = service;
        }

        /// <param name="service">The service which could not be resolved.</param>
        /// <param name="mappingName">The mapping name.</param>
        public ServiceResolutionFailedException(Type service, string mappingName) : base(
            $"Service \"{service.FullName}\" (mappingName: {mappingName}) could not be resolved.")
        {
            MappingName = mappingName;
        }

        /// <summary>
        ///     The mapping name used to resolve the service.
        /// </summary>
        public string MappingName { get; }

        /// <summary>
        ///     The service which could not be found.
        /// </summary>
        public Type Service { get; }
    }
}