namespace Rocket.API.Configuration
{
    public interface IConfigurationSection : IConfigurationElement
    {
        /// <summary>Gets the key this section occupies in its parent.</summary>
        string Key { get; }

        bool HasValue { get; }
    }
}