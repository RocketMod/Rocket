namespace Rocket.API.DependencyInjection
{
    public interface IDependencyContainer : IDependencyResolver
    {
        IDependencyContainer CreateChildContainer();

        void RegisterType<TInterface, TClass>(params string[] mappingNames) where TClass : TInterface;

        void RegisterSingletonType<TInterface, TClass>(params string[] mappingNames) where TClass : TInterface;

        void RegisterInstance<TInterface>(TInterface value, params string[] mappingNames);

        void RegisterSingletonInstance<TInterface>(TInterface value, params string[] mappingNames);
    }
}