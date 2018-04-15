namespace Rocket.API
{
    public interface IConfigurationContext
    {
        string WorkingDirectory { get; }
        string ConfigurationName { get; }
    }
}