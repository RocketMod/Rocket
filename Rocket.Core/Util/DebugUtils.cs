using System;
using System.Diagnostics;
using System.Threading;

namespace Rocket.Core.Util
{
    public class DebugUtils
    {
        public static void WaitForDebugger(bool autoBreak = true)
        {
            Console.WriteLine("Waiting for debugger");
            while (!Debugger.IsAttached)
            {
                Console.Write(".");
                Thread.Sleep(500);
            }
            Console.WriteLine();

            if(autoBreak)
                Debugger.Break();
        }
    }
}