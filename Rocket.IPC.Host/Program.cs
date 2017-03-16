using Logger = Rocket.API.Logging.Logger;
using Rocket.Core.RPC;
using System;

namespace Rocket.IPC.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                API.Environment.Initialize();
                RocketServiceHost host = new RocketServiceHost(27115);
                while (true)
                {
                    Logger.Info(Console.ReadLine());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
