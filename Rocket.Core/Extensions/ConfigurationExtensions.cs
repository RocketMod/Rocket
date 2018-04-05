using System;
using Rocket.API.Configuration;

namespace Rocket.Core.Extensions
{
    public static class ConfigurationExtensions
    {
        public static T Get<T>(this IConfiguration config, string elem)
        {
            throw new NotImplementedException();
        }

        public static bool TryGet<T>(this IConfiguration config, out T result, string elem)
        {
            result = default(T);

            try
            {
                result = Get<T>(config, elem);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}