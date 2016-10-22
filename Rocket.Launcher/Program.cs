using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Launcher
{
    class Program
    {
        static void Main()
        {
            EndpointAddress address = new EndpointAddress("http://localhost:13378/");
            R.RocketServiceClient R = new R.RocketServiceClient(new BasicHttpBinding(),address);

            while (true)
            {
                Console.ReadKey();
                R.HelloWorld();
            }

        }
    }
}
