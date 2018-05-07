namespace Rocket.API.Configuration
{
    /// <summary>
    ///     A configuration context defines where a configuration should be located and how it should be named.
    /// </summary>
    public interface IConfigurationContext
    {
        /// <summary>
        ///     The working directory for the configuration.
        /// </summary>
        string WorkingDirectory { get; }

        /// <summary>
        ///     The configuration name.<br/><br/>
        ///     <b>Must never include file endings like .json, .xml etc.</b>
        /// </summary>
        string ConfigurationName { get; }

        /// <summary>
        ///     Creates a child context.
        /// </summary>
        /// <param name="childName">The child name.</param>
        /// <returns>The child context instance.</returns>
        IConfigurationContext CreateChildConfigurationContext(string childName);
    }
}