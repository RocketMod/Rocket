using System.Threading.Tasks;

namespace Rocket.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var bootTask = new Runtime().BootstrapAsync();
            bootTask.GetAwaiter().GetResult();
        }
    }
}