namespace Rocket.API.DependencyInjection
{
    public interface IDependencyContainer : IDependencyResolver
    {
        IServiceLocator ServiceLocator { get; }
        void RegisterType<TInterface, TClass>(string mappingName = null) where TClass : TInterface;

        void RegisterSingletonType<TInterface, TClass>(string mappingName = null) where TClass : TInterface;

        void RegisterInstance<TInterface>(TInterface value, string mappingName = null);
    }
}