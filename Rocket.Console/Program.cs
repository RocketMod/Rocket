using System.Threading.Tasks;

namespace Rocket.Console
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
#if NUGET_BOOTSTRAP
            await new RocketDynamicBootstrapper().BootstrapAsync("./Rocket", false);
#else
            await new Runtime().InitAsync();
#endif
        }
    }
}