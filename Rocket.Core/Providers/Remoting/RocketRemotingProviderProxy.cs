using System;
using Rocket.API.Providers;
using Rocket.API.Providers.Remoting;

namespace Rocket.Core.Providers.Remoting
{
    [RocketProviderProxy]
    public class RocketRemotingProviderProxy : IRocketRemotingProvider
    {
        public void Load(bool isReload = false)
        {
            
        }

        public RockedCommandExecute OnExecute()
        {
            throw new NotImplementedException(); //?????
        }

        public void Unload(bool isReload = false)
        {
            
        }
    }
}