using System;
using Rocket.Core;

namespace Rocket.Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            R.Bootstrap();
            ITestThingie test = R.ServiceLocator.GetInstance<ITestThingie>();
            Console.ReadLine();
        }
    }
}
