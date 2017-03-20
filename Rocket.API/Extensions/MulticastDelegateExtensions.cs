using System;

namespace Rocket.API.Extensions
{
    public static class MulticastDelegateExtension
    {
        public static void TryInvoke(this System.MulticastDelegate theDelegate, params object[] args)
        {
            if (theDelegate == null) return;
            foreach (Delegate handler in theDelegate.GetInvocationList())
            {
                try
                {
                    handler.DynamicInvoke(args);
                }
                catch (Exception)
                {
                    //
                }
            }
        }
    }
}
