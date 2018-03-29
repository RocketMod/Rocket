using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.API
{
    public interface IImplementation
    {
        IEnumerable<string> Capabilities { get; }
        void Shutdown();
        string InstanceId { get; }
        void Reload();
    }
}
