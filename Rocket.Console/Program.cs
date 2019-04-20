using System.Linq;
using System.Threading.Tasks;

namespace Rocket.Console
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
#if NUGET_BOOTSTRAP
            await new RocketDynamicBootstrapper().BootstrapAsync("./Rocket", "Rocket.Core", args.Contains("-Pre"));
#else
            await new Runtime().InitAsync();
#endif
        }
    }
}