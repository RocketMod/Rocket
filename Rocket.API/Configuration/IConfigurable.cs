namespace Rocket.API.Configuration
{
    /// <summary>
    ///     An object which has a configuration.
    /// </summary>
    public interface IConfigurable
    {
        /// <summary>
        ///     The configuration instance.
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        ///     The default configuration.
        /// </summary>
        object DefaultConfiguration { get; }
    }
}