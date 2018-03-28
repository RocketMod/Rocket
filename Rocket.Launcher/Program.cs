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
            test.SetTheThing = true;

            ITestThingie test2 = R.ServiceLocator.GetInstance<ITestThingie>();
            Console.WriteLine(test2.SetTheThing);

            Console.ReadLine();
        }
    }
}
