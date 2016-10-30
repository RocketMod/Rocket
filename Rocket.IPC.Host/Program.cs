using Rocket.Core.IPC;
using System;

namespace Rocket.IPC.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            Core.Environment.Initialize();
            new RocketServiceHost(27115);
            Console.ReadKey();
        }
    }

}
