using Rocket.Core.IPC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

namespace Rocket.IPC.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            Rocket.Core.Environment.Initialize();
            new RocketServiceHost(27115);
            Console.ReadKey();
        }
    }

}
