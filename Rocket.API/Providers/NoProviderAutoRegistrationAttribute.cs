using System;

namespace Rocket.API.Providers
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NoProviderAutoRegistrationAttribute : Attribute
    {
        
    }
}