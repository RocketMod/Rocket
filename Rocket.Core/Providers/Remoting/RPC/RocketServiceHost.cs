using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Rocket.API.Player;
using Rocket.Core.Providers.Logging;

namespace Rocket.Core.Providers.Remoting.RPC
{
    public class RocketServiceHost
    {
        private ServiceHost serviceHost = null;
        private Uri endpoint;

        public static LongPollingEvent<RocketPlayerBase> OnPlayerConnected = new LongPollingEvent<RocketPlayerBase>();
        public static LongPollingEvent<RocketPlayerBase> OnPlayerDisconnected = new LongPollingEvent<RocketPlayerBase>();
        public static LongPollingEvent<LogMessage> OnLog = new LongPollingEvent<LogMessage>();
        public static LongPollingEvent OnShutdown = new LongPollingEvent();

        public RocketServiceHost(ushort port)
        {
            endpoint = new Uri(String.Format("http://localhost:{0}/", port));
            try
            {
                serviceHost = new ServiceHost(typeof(RocketService), endpoint);
                BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
                binding.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                binding.Security.Mode = BasicHttpSecurityMode.TransportCredentialOnly;
                serviceHost.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new RPCUserNameValidator();

                binding.CloseTimeout = new TimeSpan(0, 10, 0);
                binding.OpenTimeout = new TimeSpan(0, 10, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 10, 0);
                binding.SendTimeout = new TimeSpan(0, 10, 0);

                serviceHost.AddServiceEndpoint(typeof(IRocketService), binding, "");
#if DEBUG
                serviceHost.Description.Behaviors.Add(new ServiceMetadataBehavior());
                serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
#endif
                serviceHost.Open();

                //if (R.Implementation != null)
                //{
                //    R.Implementation.OnPlayerConnected += (IRocketPlayer player) => { OnPlayerConnected.Invoke((RocketPlayerBase)player); };
                //    R.Implementation.OnPlayerDisconnected += (IRocketPlayer player) => { OnPlayerDisconnected.Invoke((RocketPlayerBase)player); };
                //    R.Implementation.OnShutdown += () => { OnShutdown.Invoke(); };
                //}

                ////TODO double log
                //R.Logger.OnLog += (LogMessage message) => { if (message.LogLevel != LogLevel.DEBUG) OnLog.Invoke(message); };
                
                R.Logger.Info("Starting IPC at " + endpoint);
            }
            catch (Exception e)
            {
                R.Logger.Error("Failed to start IPC at " + endpoint, e);
            }
        }

        public void Stop()
        {
              if (serviceHost != null)
              {
                  serviceHost.Close();
                  serviceHost = null;
              } 
        }

    }
}
