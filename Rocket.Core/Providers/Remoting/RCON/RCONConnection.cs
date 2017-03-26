using System.Linq;
using System.Net.Sockets;

namespace Rocket.Core.Providers.Remoting.RCON
{
    public class RCONConnection
    {
        public TcpClient Client;
        public bool Authenticated;
        public bool Interactive;

        public RCONConnection(TcpClient client)
        {
            this.Client = client;
            Authenticated = false;
            Interactive = true;
        }

        public void Send(string command, bool nonewline = false)
        {
            if (Interactive)
            {
                if (nonewline == true)
                    RocketBuiltinRCONRemotingProvider.Send(Client, command);
                else
                    RocketBuiltinRCONRemotingProvider.Send(Client, command + (!command.Contains('\n') ? "\r\n" : ""));
                return;
            }
        }

        public string Read()
        {
            return RocketBuiltinRCONRemotingProvider.Read(Client);
        }

        public void Close()
        {
            this.Client.Close();
            return;
        }

        public string Address { get { return this.Client.Client.RemoteEndPoint.ToString(); } }
    }

}