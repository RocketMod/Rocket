using System.Threading.Tasks;

namespace Rocket.Console
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            await new Runtime().BootstrapAsync();
        }
    }
}