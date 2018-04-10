namespace Rocket.API.Configuration
{
    public interface IConfigurationSection : IConfigurationBase
    {
        /// <summary>Gets the key this section occupies in its parent.</summary>
        string Key { get; }

        bool IsNull { get; }
    }
}