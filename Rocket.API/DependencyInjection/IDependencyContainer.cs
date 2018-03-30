namespace Rocket.API.DependencyInjection
{
    public interface IDependencyContainer : IDependencyResolver
    {
        IDependencyContainer CreateChildContainer();
        IServiceLocator ServiceLocator { get; }
        void RegisterType<TInterface, TClass>(string mappingName = null) where TClass : TInterface;

        void RegisterSingletonType<TInterface, TClass>(string mappingName = null) where TClass : TInterface;

        void RegisterInstance<TInterface>(TInterface value, string mappingName = null);

        void RegisterSingletonInstance<TInterface>(TInterface value, string mappingName = null);
    }
}