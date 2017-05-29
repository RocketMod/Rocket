using System;

namespace Rocket.API.Utils.Debugging
{
    public static class Assert
    {
        public static void NotNull(object o, string paramName = null)
        {
#if !DEBUG
            return;
#endif

            if (o != null)
                return;

            paramName = paramName ?? "parameter";

            throw new Exception("Expected " + paramName + " to be not null!");
        }

        public static void IsTrue(bool o, string paramName)
        {
#if !DEBUG
            return;
#endif
            if (o)
                return;

            paramName = paramName ?? "parameter";

            throw new Exception("Expected " + paramName + " to be true!");
        }
    }
}