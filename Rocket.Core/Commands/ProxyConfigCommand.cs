namespace Rocket.Core.Commands {
    public class ProxyConfigCommand
    {
        public string Name { get; set; }
        public string Provider { get; set; }
        public bool IsEnabled { get; set; } = true;
    }
}