using System;
using System.Collections.Generic;

namespace Rocket.API.DependencyInjection {
    public interface IDependencyResolver {
        bool IsRegistered<T>(string mappingName = null);
        bool IsRegistered(Type type, string mappingName = null);

        T Activate<T>();
        object Activate(Type type);

        T Get<T>(string mappingName = null);
        object Get(Type serviceType, string mappingName = null);

        T Get<T>(string mappingName, params object[] parameters);
        object Get(Type serviceType, string mappingName, params object[] parameters);

        IEnumerable<T> GetAll<T>();
        IEnumerable<object> GetAll(Type type);

        IEnumerable<T> GetAll<T>(params object[] parameters);
        IEnumerable<object> GetAll(Type type, params object[] parameters);

        bool TryGet<T>(string mappingName, out T output);
        bool TryGet(Type serviceType, string mappingName, out object output);

        bool TryGet<T>(string mappingName, out T output, params object[] parameters);
        bool TryGet(Type serviceType, string mappingName, out object output, params object[] parameters);

        bool TryGetAll<T>(out IEnumerable<T> output);
        bool TryGetAll(Type type, out IEnumerable<object> output);

        bool TryGetAll<T>(out IEnumerable<T> output, params object[] parameters);
        bool TryGetAll(Type type, out IEnumerable<object> output, params object[] parameters);
    }
}