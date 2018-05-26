using System;

namespace Rocket.Core.ServiceProxies
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DontProxyAttribute: Attribute
    {
        
    }
}