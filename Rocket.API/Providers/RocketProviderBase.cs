using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Rocket.API.Providers
{
    public abstract class RocketProviderBase : IRocketProviderBase
    {
        public abstract void Load();

        public abstract void Unload();
    }
}
