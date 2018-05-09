using System;

namespace Rocket.API.Configuration
{
    /// <summary>
    ///     Thrown when trying to save or reload a configuration but the context was not set yet.
    /// </summary>
    public class ConfigurationContextNotSetException : Exception
    {
        /// <param name="configuration">The configuration whichs context was not set.</param>
        public ConfigurationContextNotSetException(IConfiguration configuration) : base(
            "The configuration context was not set.")
        {
            Configuration = configuration;
        }

        /// <summary>
        ///     The configuration whichs context was not set.
        /// </summary>
        public IConfiguration Configuration { get; }
    }
}