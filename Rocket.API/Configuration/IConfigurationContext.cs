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
        ///     The configuration name.<br /><br />
        ///     <b>Must never include file endings like .json, .xml etc.</b>
        /// </summary>
        string ConfigurationName { get; }
    }
}