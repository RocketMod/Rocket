using System.Diagnostics;
using System.Threading;

namespace Rocket.Core.Util
{
    public class DebugUtils
    {
        public static void WaitForDebugger(bool autoBreak = true)
        {
            while (!Debugger.IsAttached)
                Thread.Sleep(500);

            if(autoBreak)
                Debugger.Break();
        }
    }
}