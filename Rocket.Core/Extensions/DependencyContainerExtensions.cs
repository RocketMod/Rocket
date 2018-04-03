using System.Collections.Generic;
using System.Linq;
using Rocket.API.DependencyInjection;
using Rocket.API.Handlers;

namespace Rocket.Core.Extensions
{
    public static class DependencyContainerExtensions
    {
        public static List<T> GetHandlers<T>(this IDependencyContainer container) where T : IHandler
        {
            var instances = container.GetAll<T>().ToList();
            return HandlerPriorityComparer.Sort(instances);
        }
    }
}