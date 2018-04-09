using System.Collections.Generic;

namespace Rocket.API.Configuration
{
    public interface IConfigurationBase
    {
        /// <summary>Gets or sets a configuration value.</summary>
        /// <param name="key">The configuration key.</param>
        /// <returns>The configuration value.</returns>
        string this[string key] { get; set; }

        /// <summary>
        /// Gets a configuration sub-section with the specified key.
        /// </summary>
        /// <param name="key">The key of the configuration section.</param>
        /// <returns>The <see cref="IConfigurationSection" />.</returns>
        /// <remarks>
        ///     This method will never return <c>null</c>. If no matching sub-section is found with the specified key,
        ///     an empty <see cref="T:IConfigurationSection" /> will be returned.
        /// </remarks>
        IConfigurationSection GetSection(string key);

        /// <summary>
        /// Gets the immediate descendant configuration sub-sections.
        /// </summary>
        /// <returns>The configuration sub-sections.</returns>
        IEnumerable<IConfigurationSection> GetChildren();

        void Set(object o);

        T Get<T>();

        bool TryGet<T>(out T value);
    }
}