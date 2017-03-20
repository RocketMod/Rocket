using Rocket.API;
using Rocket.API.Player;

namespace Rocket.Core.Providers.Remoting.RPC
{
    public class RocketService : IRocketService
    {
        public void Disconnect(bool shutdown)
        {
            if (shutdown) R.Implementation.Shutdown();
        }

        public void Execute(string command)
        {
            R.Execute(new ConsolePlayer(), command);
        }

        public bool Test()
        {
            return true;
        }

       /* public bool OnShutdown()
        {
            return RocketServiceHost.OnShutdown.Request();
        }
        public RocketPlayer OnPlayerConnected()
        {
            return RocketServiceHost.OnPlayerConnected.Request();
        }

        public RocketPlayer OnPlayerDisconnected()
        {
            return RocketServiceHost.OnPlayerDisconnected.Request();
        }

        public Queue<LogMessage> OnLog()
        {
            return RocketServiceHost.OnLog.RequestAll();
        }*/
    }

}
