using System.ServiceModel;

namespace Rocket.Core.Providers.Remoting.RPC
{
    [ServiceContract]
    public interface IRocketService
    {
        [OperationContract]
        void Execute(string command);

        [OperationContract]
        bool Test();

        [OperationContract]
        void Disconnect(bool shutdown);
        /*
        [OperationContract]
        RocketPlayer OnPlayerConnected();

        [OperationContract]
        RocketPlayer OnPlayerDisconnected();

        [OperationContract]
        bool OnShutdown();

        [OperationContract]
        Queue<LogMessage> OnLog();*/

    }
}
