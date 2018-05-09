using System;

namespace Rocket.API.Configuration
{
    /// <summary>
    ///     Thrown when accessing elements of a configuration but it was not loaded.
    /// </summary>
    public class ConfigurationNotLoadedException : Exception
    {
        /// <param name="configuration">The configuration which was not loaded yet.</param>
        public ConfigurationNotLoadedException(IConfiguration configuration) : base(
            "The configuration or section has not been loaded.")
        {
            Configuration = configuration;
        }

        /// <summary>
        ///     The configuration which was not loaded yet.
        /// </summary>
        public IConfiguration Configuration { get; }
    }
}