using System.Collections.Generic;
using System.Linq;

namespace Rocket.Core.ServiceProxies
{
    public static class ServicePriorityComparer
    {
        public static int Compare(ServicePriority a, ServicePriority b, bool highestFirst)
            => highestFirst ? Compare(b, a) : Compare(a, b);

        public static int Compare(ServicePriority a, ServicePriority b)
            => ((int) a).CompareTo((int) b);

        public static void Sort<T>(List<T> objects)
        {
            Sort(objects, false);
        }

        public static void Sort<T>(List<T> objects, bool highestFirst)
        {
            objects.Sort((a, b) => Compare(GetPriority(a), GetPriority(b), highestFirst));
        }

        public static ServicePriority GetPriority(object a)
        {
            ServicePriorityAttribute serviceAttribute = (ServicePriorityAttribute)
                a.GetType()
                 .GetCustomAttributes(
                     typeof(ServicePriorityAttribute), true)
                 .FirstOrDefault()
                ?? new ServicePriorityAttribute
                {
                    Priority = ServicePriority.Normal
                };

            return serviceAttribute.Priority;
        }
    }
}