using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Rocket.Core.IPC
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IRocketServiceCallback))]
    public interface IRocketService
    {
        [OperationContract]
        void HelloWorld();

        [OperationContract()]
        void Subscribe();
    }

    public interface IRocketServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void NotifyPlayerConnected(API.IRocketPlayer player);

        [OperationContract(IsOneWay = true)]
        void NotifyPlayerDisconnected(API.IRocketPlayer player);
    }
}
