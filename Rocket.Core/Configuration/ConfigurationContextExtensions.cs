using Rocket.API.Configuration;

namespace Rocket.Core.Configuration
{
    public static class ConfigurationContextExtensions
    {
        /// <summary>
        ///     Creates a child context.
        /// </summary>
        /// <param name="context">The configuration context.</param>
        /// <param name="childName">The child name.</param>
        /// <returns>The child context instance.</returns>
        public static IConfigurationContext CreateChildConfigurationContext(
            this IConfigurationContext context, string childName) => new ConfigurationContext(context, childName);
    }
}