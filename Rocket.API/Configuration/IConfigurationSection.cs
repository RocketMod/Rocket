namespace Rocket.API.Configuration
{
    public interface IConfigurationSection : IConfigurationBase
    {
        /// <summary>Gets the key this section occupies in its parent.</summary>
        string Key { get; }

        /// <summary>
        /// Gets the full path to this section within the <see cref="IConfigurationBase" />.
        /// </summary>
        string Path { get; }

        /// <summary>Gets or sets the section value.</summary>
        string Value { get; set; }
    }
}