using System.Collections.Generic;
using System.Linq;
using Rocket.API.Handlers;
using Rocket.API.Ioc;

namespace Rocket.Core.Extensions
{
    public static class DependencyContainerExtensions
    {
        public static List<T> GetHandlers<T>(this IDependencyContainer container) where T : IHandler
        {
            List<T> instances = container.GetAll<T>().ToList();
            return HandlerPriorityComparer.Sort(instances);
        }
    }
}