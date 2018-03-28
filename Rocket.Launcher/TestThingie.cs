using Rocket.Core;

namespace Rocket.Launcher
{
    public class TestThingie : ITestThingie
    {
        public TestThingie(ILog logger)
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
