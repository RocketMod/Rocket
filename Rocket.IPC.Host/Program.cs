using System;
using Rocket.Core;
using Rocket.Core.Providers.Remoting.RPC;

namespace Rocket.IPC.Host
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                RocketServiceHost host = new RocketServiceHost(27115);
                while (true)
                {
                    R.Logger.Info(Console.ReadLine());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

}
