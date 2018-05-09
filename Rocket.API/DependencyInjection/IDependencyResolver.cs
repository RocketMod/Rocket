using System;
using System.Collections.Generic;

namespace Rocket.API.DependencyInjection
{
    /// <summary>
    ///     Resolves dependencies.
    /// </summary>
    public interface IDependencyResolver
    {
        /// <summary>
        ///     Checks if an implementation exists for the given service.
        /// </summary>
        /// <typeparam name="T">The service to check.</typeparam>
        /// <param name="mappingName">The mapping names.</param>
        /// <returns><b>true</b> if the service was registered; otherwise, <b>false</b>.</returns>
        bool IsRegistered<T>(string mappingName = null);

        /// <summary>
        ///     Checks if an implementation exists for the given service.
        /// </summary>
        /// <param name="type">The service to check.</param>
        /// <param name="mappingName">The mapping names.</param>
        /// <returns><b>true</b> if the service was registered; otherwise, <b>false</b>.</returns>
        bool IsRegistered(Type type, string mappingName = null);

        /// <summary>
        ///     Activates an instance for the given service.
        /// </summary>
        /// <typeparam name="T">The service to activate.</typeparam>
        /// <returns>the activated service instance.</returns>
        T Activate<T>();

        /// <summary>
        ///     Activates an instance for the given service.
        /// </summary>
        /// <param name="type">The service to activate.</param>
        /// <returns>the activated service instance.</returns>
        object Activate(Type type);

        /// <summary>
        ///     Gets the primary service implementation instance or the <see cref="IServiceProxy">service proxy</see> if one
        ///     exists.
        /// </summary>
        /// <typeparam name="T">The service to get the implementation instance of.</typeparam>
        /// <param name="mappingName">The mapping name.</param>
        /// <returns>
        ///     the primary service instance implementation instance or the <see cref="IServiceProxy">service proxy</see> if
        ///     one exists.
        /// </returns>
        /// <exception cref="ServiceResolutionFailedException">When the service could not be resolved.</exception>
        T Resolve<T>(string mappingName = null);

        /// <summary>
        ///     Gets the primary service implementation instance or the <see cref="IServiceProxy">service proxy</see> if one
        ///     exists.
        /// </summary>
        /// <param name="serviceType">The service to get the implementation instance of.</param>
        /// <param name="mappingName">The mapping name.</param>
        /// <returns>
        ///     the primary service implementation instance or the <see cref="IServiceProxy">service proxy</see> if one
        ///     exists.
        /// </returns>
        /// <exception cref="ServiceResolutionFailedException">When the service could not be resolved.</exception>
        object Resolve(Type serviceType, string mappingName = null);

        /// <summary>
        ///     Gets the primary service implementation instance or the <see cref="IServiceProxy">service proxy</see> if one
        ///     exists.
        /// </summary>
        /// <typeparam name="T">The service to get the implementation instance of.</typeparam>
        /// <param name="mappingName">The mapping name.</param>
        /// <param name="parameters">The service parameters.</param>
        /// <returns>
        ///     the primary service implementation instance or the <see cref="IServiceProxy">service proxy</see> if one
        ///     exists.
        /// </returns>
        /// <exception cref="ServiceResolutionFailedException">When the service could not be resolved.</exception>
        T Resolve<T>(string mappingName, params object[] parameters);

        /// <summary>
        ///     Gets the primary service implementation instance or the <see cref="IServiceProxy">service proxy</see> if one
        ///     exists.
        /// </summary>
        /// <param name="serviceType">The service to get the implementation instance of.</param>
        /// <param name="mappingName">The mapping names.</param>
        /// <param name="parameters">The service parameters.</param>
        /// <returns>
        ///     the primary service implementation instance or the <see cref="IServiceProxy">service proxy</see> if one
        ///     exists.
        /// </returns>
        /// <exception cref="ServiceResolutionFailedException">When the service could not be resolved.</exception>
        object Resolve(Type serviceType, string mappingName, params object[] parameters);

        /// <summary>
        ///     Gets all implementation instances for the given service.
        /// </summary>
        /// <typeparam name="T">The service to get the implementation instances of.</typeparam>
        /// <returns>all implementation instances for the given service.</returns>
        IEnumerable<T> ResolveAll<T>();

        /// <summary>
        ///     Gets all implementation instances for the given service.
        /// </summary>
        /// <param name="type">The service to get the implementation instances of.</param>
        /// <returns>all implementation instances for the given service.</returns>
        IEnumerable<object> ResolveAll(Type type);

        /// <summary>
        ///     Gets all implementation instances for the given service.
        /// </summary>
        /// <typeparam name="T">The service to get the implementation instances of.</typeparam>
        /// <param name="parameters">The service parameters.</param>
        /// <returns>all implementation instances for the given service.</returns>
        IEnumerable<T> ResolveAll<T>(params object[] parameters);

        /// <summary>
        ///     Gets all implementation instances for the given service.
        /// </summary>
        /// <param name="type">The service to get the implementation instances of.</param>
        /// <param name="parameters">The service parameters.</param>
        /// <returns>all implementation instances for the given service.</returns>
        IEnumerable<object> ResolveAll(Type type, params object[] parameters);

        /// <summary>
        ///     Tries to get the primary service implementation instance or the service proxy if one exists.
        /// </summary>
        /// <typeparam name="T">The service to get the implementation instance of.</typeparam>
        /// <param name="mappingName">The mapping name.</param>
        /// <param name="serviceInstance">The service implementation instance if it was resolved; otherwise, <b>null</b>.</param>
        /// <returns><b>true</b> if the service was resolved; otherwise, <b>false</b>.</returns>
        bool TryResolve<T>(string mappingName, out T serviceInstance);

        /// <summary>
        ///     Tries to get the primary service implementation instance or the <see cref="IServiceProxy">service proxy</see> if
        ///     one exists.
        /// </summary>
        /// <param name="serviceType">The service to get the implementation instance of.</param>
        /// <param name="mappingName">The mapping name.</param>
        /// <param name="serviceInstance">The service implementation instance if it was resolved; otherwise, <b>null</b>.</param>
        /// <returns><b>true</b> if the service was resolved; otherwise, <b>false</b>.</returns>
        bool TryResolve(Type serviceType, string mappingName, out object serviceInstance);

        /// <summary>
        ///     Tries to get the primary service implementation instance or the service proxy if one exists.
        /// </summary>
        /// <typeparam name="T">The service to get the implementation instance of.</typeparam>
        /// <param name="mappingName">The mapping name.</param>
        /// <param name="serviceInstance">The service implementation instance if it was resolved; otherwise, <b>null</b>.</param>
        /// <param name="parameters">The service parameters.</param>
        /// <returns><b>true</b> if the service was resolved; otherwise, <b>false</b>.</returns>
        bool TryResolve<T>(string mappingName, out T serviceInstance, params object[] parameters);

        /// <summary>
        ///     Tries to get the primary service implementation instance or the <see cref="IServiceProxy">service proxy</see> if
        ///     one exists.
        /// </summary>
        /// <param name="serviceType">The service to get the implementation instance of.</param>
        /// <param name="mappingName">The mapping name.</param>
        /// <param name="serviceInstance">The service implementation instance if it was resolved; otherwise, <b>null</b>.</param>
        /// <param name="parameters">The service parameters.</param>
        /// <returns><b>true</b> if the service was resolved; otherwise, <b>false</b>.</returns>
        bool TryResolve(Type serviceType, string mappingName, out object serviceInstance, params object[] parameters);
    }
}