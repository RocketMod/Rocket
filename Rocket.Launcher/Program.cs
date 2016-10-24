using Rocket.Launcher.R;
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
            RocketServiceClient R = new RocketServiceClient();

            while (true)
            {
                Console.ReadKey();
                R.Client.HelloWorld();
            }
        }

    }
    public class RocketServiceClient : IRocketServiceCallback
    {
        public R.RocketServiceClient Client;
        public RocketServiceClient(string url = "http://localhost:27115/")
        {
            EndpointAddress address = new EndpointAddress(url);
            Client = new R.RocketServiceClient(new InstanceContext(this), new WSHttpBinding(), address);
            Client.Subscribe();
        }

        public delegate void PlayerJoined(string name);
        public event PlayerJoined OnPlayerJoined;
        void IRocketServiceCallback.NotifyPlayerJoined(string guestName)
        {
            OnPlayerJoined?.Invoke(guestName);
        }

        public delegate void PlayerLeft(string name);
        public event PlayerLeft OnPlayerLeft;
        void IRocketServiceCallback.NotifyPlayerLeft(string guestName)
        {
            OnPlayerLeft?.Invoke(guestName);
        }
    }
}
