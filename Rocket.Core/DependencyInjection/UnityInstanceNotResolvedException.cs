using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.API.DependencyInjection
{
    public sealed class UnityInstanceNotResolvedException : Exception
    {
        public readonly Type RequestedInterface;
        public readonly string Mapping;
        
        public UnityInstanceNotResolvedException(Type _interface, string mapping) 
            : base(string.Format("No implementation was found for {0} under the {1} mapping.", _interface.Name, (mapping == null) ? "default" : mapping))
        {
            RequestedInterface = _interface;
            Mapping = mapping;
        }
        
        public UnityInstanceNotResolvedException(Type _interface)
            : base(string.Format("No implementations were found for {0}.", _interface.Name))
        {
            RequestedInterface = _interface;
            Mapping = null;
        }
    }
}
