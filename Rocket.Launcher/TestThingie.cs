using Rocket.API.Logging;

namespace Rocket.Launcher
{
    public class TestThingie : ITestThingie
    {
        public TestThingie(ILogger logger)
        {
            logger.Info("Hallo");
        }

        public bool SetTheThing { get; set; } = false;
    }

    public interface ITestThingie
    {
        bool SetTheThing { get; set; }
    }
}
