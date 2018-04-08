using Rocket.API;

namespace Rocket.Core
{
    //Subject to change
    public class EnvironmentContext : IEnvironmentContext
    {
        public string WorkingDirectory { get; set; }
        public string Name { get; set; }
    }
}