using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rocket.IOC
{
    public interface  IDependencyRegistrator
    {
        void Register(IDependencyContainer container, IDependencyResolver resolver);
    }
}
