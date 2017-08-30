using System;
using System.IdentityModel.Selectors;
using System.ServiceModel;

namespace Rocket.Core.Remoting.RPC
{
    public class RPCUserNameValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            if (null == userName || null == password)
            {
                throw new ArgumentNullException();
            }

#if DEBUG
            if (!(userName == "admin" && password == "asdf"))
            {
                throw new FaultException("Unknown Username or Incorrect Password");
            }
#else
            if (!(userName == R.Settings.Instance.RPC.Username && password == R.Settings.Instance.RPC.Password))
            {
                throw new FaultException("Unknown Username or Incorrect Password");
            }
#endif

        }
    }
}
