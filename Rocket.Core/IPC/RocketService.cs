using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logger = Rocket.API.Logging.Logger;

namespace Rocket.Core.IPC
{
    public class RocketService : IRocketService
    {
        public void HelloWorld()
        {
            Logger.Info("Hello World!");
            Console.WriteLine("Hello World!");
        }
    }
}
