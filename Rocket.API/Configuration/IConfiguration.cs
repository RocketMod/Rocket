namespace Rocket.API.Configuration
{
    /// <summary>
    ///     A configuration provider. Also represents the root node of the configuration tree.
    /// </summary>
    public interface IConfiguration : IConfigurationElement
    {
        /// <summary>
        ///     Checks if the configuration was loaded.
        /// </summary>
        bool IsLoaded { get; }

        /// <summary>
        ///     Checks if the configuration for the given context exists.
        /// </summary>
        /// <param name="context">The context to check.</param>
        /// <returns><b>true</b> if the configuration exists; otherwise, <b>false</b>.</returns>
        bool Exists(IConfigurationContext context);

        /// <summary>
        ///     Loads the configuration from the given context. 
        ///     If the configuration does not exist, it will create oen from the default configuration.<br/><br/>
        ///     <b><see cref="ConfigurationContext"/> must be set before.</b> 
        /// </summary>
        /// <param name="defaultConfiguration">The default configuration to be used if the configuration does not exist already.</param>
        void Load(object defaultConfiguration);

        /// <summary>
        ///     The configuration context.
        /// </summary>
        IConfigurationContext ConfigurationContext { get; set; }

        /// <summary>
        ///     Loads a new empty configuration without any context.
        /// </summary>
        void LoadEmpty();

        /// <summary>
        ///     Loads the configuration from the given object.
        /// </summary>
        /// <param name="o">The object to load as configuration.</param>
        void LoadFromObject(object o);

        /// <summary>
        ///     Reloads the configuration from the provided context.<br/><br/>
        ///     <b><see cref="ConfigurationContext"/> must be set before.</b> 
        /// </summary>
        void Reload();

        /// <summary>
        ///     Saves the configuration to the provided context.<br/><br/>
        ///     <b><see cref="ConfigurationContext"/> must be set before.</b> 
        /// </summary>
        void Save();
    }
}