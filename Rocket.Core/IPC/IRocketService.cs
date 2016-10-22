using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace Rocket.Core.IPC
{
    [ServiceContract]
    public interface IRocketService
    {
        [OperationContract]
        void HelloWorld();
    }
}
