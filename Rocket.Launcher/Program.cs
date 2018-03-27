using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
