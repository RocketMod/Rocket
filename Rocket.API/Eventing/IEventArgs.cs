using System.Collections.Generic;

namespace Rocket.API.Eventing
{
    public interface IEventArgs
    {
        Dictionary<string, object> Arguments { get; }
    }
}