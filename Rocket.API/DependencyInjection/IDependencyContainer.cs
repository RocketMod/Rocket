using System;

namespace Rocket.API.DependencyInjection
{
    /// <summary>
    ///     A dependency container. See
    ///     <a href="https://msdn.microsoft.com/en-us/library/ff921087.aspx">https://msdn.microsoft.com/en-us/library/ff921087.aspx</a>
    ///     for more.
    /// </summary>
    public interface IDependencyContainer : IDependencyResolver, IDisposable
    {
        /// <summary>
        ///     The parent container.
        /// </summary>
        IDependencyContainer ParentContainer { get; }

        /// <summary>
        ///     Creates a child container.
        /// </summary>
        /// <returns>The created child container.</returns>
        IDependencyContainer CreateChildContainer();

        /// <summary>
        ///     Registers a service implementation. These implementation are not be shared between components.
        /// </summary>
        /// <typeparam name="TInterface">The service interface.</typeparam>
        /// <typeparam name="TClass">The service implementation.</typeparam>
        /// <param name="mappingNames">The mapping names. Include <b>null</b> in mapping names to override default provider.</param>
        void AddTransient<TInterface, TClass>(params string[] mappingNames) where TClass : TInterface;

        /// <summary>
        ///     Registers a singleton service implementation. Singleton implementation instances are shared between all components.
        /// </summary>
        /// <typeparam name="TInterface">The service interface.</typeparam>
        /// <typeparam name="TClass">The service implementation.</typeparam>
        /// <param name="mappingNames">The mapping names. Include <b>null</b> in mapping names to override default provider.</param>
        void AddSingleton<TInterface, TClass>(params string[] mappingNames) where TClass : TInterface;

        /// <summary>
        ///     Registers a service implementation. These implementation are not be shared between components.
        /// </summary>
        /// <typeparam name="TInterface">The service interface.</typeparam>
        /// <param name="value">The service implementation instance.</param>
        /// <param name="mappingNames">The mapping names. Include <b>null</b> in mapping names to override default provider.</param>
        void AddTransient<TInterface>(TInterface value, params string[] mappingNames);

        /// <summary>
        ///     Registers a singleton service implementation. Singleton implementation instances are shared between all components.
        /// </summary>
        /// <typeparam name="TInterface">The service interface.</typeparam>
        /// <param name="value">The service implementation instance.</param>
        /// <param name="mappingNames">The mapping names. Include <b>null</b> in mapping names to override default provider.</param>
        void AddSingleton<TInterface>(TInterface value, params string[] mappingNames);

        /// <summary>
        ///     Unregisters a type.
        /// </summary>
        /// <typeparam name="T">The type to unregister.</typeparam>
        /// <param name="mappingNames">The mapping names to unregister. If null or empty it will unregister everything.</param>
        void Remove<T>(params string[] mappingNames);

        /// <summary>
        ///     Unregisters a type.
        /// </summary>
        /// <param name="type">The type to unregister.</param>
        /// <param name="mappingNames">The mapping names to unregister. If null or empty it will unregister everything.</param>
        void Remove(Type type, params string[] mappingNames);
    }
}