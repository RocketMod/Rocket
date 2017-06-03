using System;

namespace Rocket.API.Providers
{
    //todo needs implementation
    public class ProviderDependencyAttribute : Attribute
    {
        public string Dependency { get; }
        public ProviderDependencyAttribute(Type type)
        {
            Dependency = type.FullName;
        }

        public ProviderDependencyAttribute(string type)
        {
            Dependency = type;
        }
    }
}