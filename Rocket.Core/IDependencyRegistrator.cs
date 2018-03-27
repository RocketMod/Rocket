using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rocket.Core
{
    public interface  IDependencyRegistrator
    {
        void Register(IDependencyContainer container, IDependencyResolver resolver);
    }
}
