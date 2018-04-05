namespace Rocket.API.Configuration
{
    public interface IConfigurationRoot : IConfiguration
    {
        /// <summary>
        /// Force the configuration values to be reloaded from the underlying <see cref="IConfigurationProvider" />s.
        /// </summary>
        void Reload();
    }
}