namespace Rocket.API {
    // Temporary class; subject to change
    public interface IEnvironmentContext
    {
        string WorkingDirectory { get; }
        string Name { get; }
    }
}