using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.API.Providers
{
    public interface IProviderManager
    {
        T CreateInstance<T>(params object[] customArguments);
        T Call<T>(object context, string methodName, params object[] customArguments);
        T GetProvider<T>() where T : class;
        object GetProvider(Type providerDefinitionType);
        void Reload();
    }
}
