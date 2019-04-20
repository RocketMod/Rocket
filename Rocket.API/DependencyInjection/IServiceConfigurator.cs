namespace Rocket.API.DependencyInjection
{
    /// <summary>
    ///     Registers services. The dependency registrator is automatically constructed and called before any plugin loads.
    /// </summary>
    public interface IServiceConfigurator
    {
        /// <summary>
        ///     Registers services.
        /// </summary>
        /// <param name="container">The dependency container.</param>
        void ConfigureServices(IDependencyContainer container);
    }
}