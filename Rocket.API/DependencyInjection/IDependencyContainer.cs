using System;

namespace Rocket.API.DependencyInjection
{
    /// <summary>
    ///     A dependency container. See <a href="https://msdn.microsoft.com/en-us/library/ff921087.aspx">https://msdn.microsoft.com/en-us/library/ff921087.aspx</a> for more.
    /// </summary>
    public interface IDependencyContainer : IDependencyResolver, IDisposable
    {
        /// <summary>
        ///     Creates a child container.
        /// </summary>
        /// <returns>The created child container.</returns>
        IDependencyContainer CreateChildContainer();

        /// <summary>
        /// The parent container.
        /// </summary>
        IDependencyContainer ParentContainer { get; }

        /// <summary>
        ///     Registers a service implementation. These implementation are not be shared between components.
        /// </summary>
        /// <typeparam name="TInterface">The service interface.</typeparam>
        /// <typeparam name="TClass">The service implementation.</typeparam>
        /// <param name="mappingNames">The mapping names. Include <b>null</b> in mapping names to override default provider.</param>
        void RegisterType<TInterface, TClass>(params string[] mappingNames) where TClass : TInterface;

        /// <summary>
        ///     Registers a singleton service implementation. Singleton implementation instances are shared between all components.
        /// </summary>
        /// <typeparam name="TInterface">The service interface.</typeparam>
        /// <typeparam name="TClass">The service implementation.</typeparam>
        /// <param name="mappingNames">The mapping names. Include <b>null</b> in mapping names to override default provider.</param>
        void RegisterSingletonType<TInterface, TClass>(params string[] mappingNames) where TClass : TInterface;

        /// <summary>
        ///     Registers a service implementation. These implementation are not be shared between components.
        /// </summary>
        /// <typeparam name="TInterface">The service interface.</typeparam>
        /// <param name="value">The service implementation instance.</param>
        /// <param name="mappingNames">The mapping names. Include <b>null</b> in mapping names to override default provider.</param>
        void RegisterInstance<TInterface>(TInterface value, params string[] mappingNames);

        /// <summary>
        ///     Registers a singleton service implementation. Singleton implementation instances are shared between all components.
        /// </summary>
        /// <typeparam name="TInterface">The service interface.</typeparam>
        /// <param name="value">The service implementation instance.</param>
        /// <param name="mappingNames">The mapping names. Include <b>null</b> in mapping names to override default provider.</param>
        void RegisterSingletonInstance<TInterface>(TInterface value, params string[] mappingNames);

        /// <summary>
        ///     Unregisters a type.
        /// </summary>
        /// <typeparam name="T">The type to unregister.</typeparam>
        /// <param name="mappingNames">The mapping names to unregister. If null or empty it will unregister everything.</param>
        void UnregisterType<T>(params string[] mappingNames);

        /// <summary>
        ///     Unregisters a type.
        /// </summary>
        /// <param name="type">The type to unregister.</param>
        /// <param name="mappingNames">The mapping names to unregister. If null or empty it will unregister everything.</param>
        void UnregisterType(Type type, params string[] mappingNames);

        /// <inheritdoc />
        void Dispose();
    }
}