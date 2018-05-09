namespace Rocket.API.DependencyInjection
{
    /// <summary>
    ///     Registers services. The dependency registrator is automatically constructed and called before any plugin loads.
    /// </summary>
    public interface IDependencyRegistrator
    {
        /// <summary>
        ///     Registers services.
        /// </summary>
        /// <param name="container">The dependency container.</param>
        /// <param name="resolver">The dependency resolver.</param>
        void Register(IDependencyContainer container, IDependencyResolver resolver);
    }
}