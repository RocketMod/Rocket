using Rocket.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
