using System.Collections.Generic;
using System.Linq;

namespace Rocket.API.Handlers
{
    public static class HandlerPriorityComparer
    {
        public static int Compare(HandlerPriority a, HandlerPriority b) => ((int) a).CompareTo((int) b);

        public static List<T> Sort<T>(List<T> objects)
        {
            List<T> cpy = objects.ToList();
            cpy.Sort((a, b) => Compare(GetPriority(a), GetPriority(b)));
            return cpy;
        }

        private static HandlerPriority GetPriority(object a)
        {
            HandlerPriorityAttribute handlerAttribute = (HandlerPriorityAttribute)
                a.GetType()
                 .GetCustomAttributes(
                     typeof(HandlerPriorityAttribute), true)
                 .FirstOrDefault()
                ?? new HandlerPriorityAttribute
                {
                    Priority = HandlerPriority.Normal
                };

            return handlerAttribute.Priority;
        }
    }
}