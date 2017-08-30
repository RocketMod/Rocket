using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.API
{
    public interface ICommandOutput
    {
        void Print(string message);
    }
}
