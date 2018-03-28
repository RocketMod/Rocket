using Rocket.Core;

namespace Rocket.Launcher
{
    public class TestThingie : ITestThingie
    {
        public TestThingie(ILog logger)
        {
            logger.Info("test");
        }
    }

    public interface ITestThingie
    {
    }
}
