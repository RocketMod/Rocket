namespace Rocket.API.Configuration
{
    /// <summary>
    ///     Represents a child configuration section.
    /// </summary>
    public interface IConfigurationSection : IConfigurationElement
    {
        /// <summary>
        ///     The sections path key.
        /// </summary>
        string Key { get; }

        /// <summary>
        ///     Checks if the section has any value.
        /// </summary>
        bool HasValue { get; }
    }
}