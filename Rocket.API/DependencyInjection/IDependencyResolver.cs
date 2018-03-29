using System;
using System.Collections.Generic;

namespace Rocket.API.DependencyInjection
{
    public interface IDependencyResolver
    {
        bool IsRegistered<T>(string mappingName = null);
        bool IsRegistered(Type type, string mappingName = null);

        T Activate<T>();
        object Activate(Type type);

        T Get<T>(string mappingName = null);
        object Get(Type serviceType, string mappingName = null);

        T Get<T>(params object[] parameters);
        object Get(Type serviceType, params object[] parameters);

        T Get<T>(string mapping, params object[] parameters);
        object Get(Type serviceType, string mappingName, params object[] parameters);

        IEnumerable<T> GetAll<T>();
        IEnumerable<object> GetAll(Type type);
        
        IEnumerable<T> GetAll<T>(params object[] parameters);
        IEnumerable<object> GetAll(Type type, params object[] parameters);


        T TryGet<T>(string mappingName = null);
        object TryGet(Type serviceType, string mappingName = null);

        T TryGet<T>(params object[] parameters);
        object TryGet(Type serviceType, params object[] parameters);

        T TryGet<T>(string mappingName, params object[] parameters);
        object TryGet(Type serviceType, string mappingName, params object[] parameters);

        IEnumerable<T> TryGetAll<T>();
        IEnumerable<object> TryGetAll(Type type);

        IEnumerable<T> TryGetAll<T>(params object[] parameters);
        IEnumerable<object> TryGetAll(Type type, params object[] parameters);
    }
}