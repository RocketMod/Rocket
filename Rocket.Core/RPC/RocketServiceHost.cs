using Rocket.API;
using Rocket.API.Logging;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Logger = Rocket.API.Logging.Logger;
namespace Rocket.Core.RPC
{
    public class RocketServiceHost
    {
        private ServiceHost serviceHost = null;
        private Uri endpoint;

        public static LongPollingEvent<RocketPlayer> OnPlayerConnected = new LongPollingEvent<RocketPlayer>();
        public static LongPollingEvent<RocketPlayer> OnPlayerDisconnected = new LongPollingEvent<RocketPlayer>();
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

                if (R.Implementation != null)
                {
                    R.Implementation.OnPlayerConnected += (IRocketPlayer player) => { OnPlayerConnected.Invoke((RocketPlayer)player); };
                    R.Implementation.OnPlayerDisconnected += (IRocketPlayer player) => { OnPlayerDisconnected.Invoke((RocketPlayer)player); };
                    R.Implementation.OnShutdown += () => { OnShutdown.Invoke(); };
                }

                Logger.OnLog += (LogMessage message) => { if (message.LogLevel != LogLevel.DEBUG) ; OnLog.Invoke(message); };
                
                Logger.Info("Starting IPC at " + endpoint);
            }
            catch (Exception e)
            {
                Logger.Error("Failed to start IPC at " + endpoint, e);
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
