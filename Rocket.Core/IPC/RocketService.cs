using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Logger = Rocket.API.Logging.Logger;

namespace Rocket.Core.IPC
{
    public class RocketService : IRocketService
    {
        public RocketService()
        {
            R.Implementation.OnPlayerConnected += (API.IRocketPlayer player) =>
            {
                foreach (IRocketServiceCallback callback in callbacks)
                {
                    callback.NotifyPlayerConnected(player);
                };
            };

            R.Implementation.OnPlayerDisconnected += (API.IRocketPlayer player) =>
            {
                foreach (IRocketServiceCallback callback in callbacks)
                {
                    callback.NotifyPlayerDisconnected(player);
                };
            };
        }

        private void Implementation_OnPlayerJoined(API.IRocketPlayer player)
        {
            throw new NotImplementedException();
        }

        public void HelloWorld()
        {
            Logger.Info("Hello World!");
            Console.WriteLine("Hello World!");
        }

        private List<IRocketServiceCallback> callbacks = new List<IRocketServiceCallback>();
        public void Subscribe()
        {
            IRocketServiceCallback subscriber = OperationContext.Current.GetCallbackChannel<IRocketServiceCallback>();

            if (!callbacks.Contains(subscriber)){
                callbacks.Add(subscriber);
            }
        }
    }

}
