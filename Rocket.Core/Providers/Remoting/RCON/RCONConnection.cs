using System.Linq;
using System.Net.Sockets;

namespace Rocket.Core.Providers.Remoting.RCON
{
    public class RconConnection
    {
        public TcpClient Client;
        public bool Authenticated;
        public bool Interactive;

        public RconConnection(TcpClient client)
        {
            Client = client;
            Authenticated = false;
            Interactive = true;
        }

        public void Send(string command, bool nonewline = false)
        {
            if (Interactive)
            {
                if (nonewline == true)
                    RocketBuiltinRconRemotingProvider.Send(Client, command);
                else
                    RocketBuiltinRconRemotingProvider.Send(Client, command + (!command.Contains('\n') ? "\r\n" : ""));
                return;
            }
        }

        public string Read()
        {
            return RocketBuiltinRconRemotingProvider.Read(Client);
        }

        public void Close()
        {
            Client.Close();
            return;
        }

        public string Address { get { return Client.Client.RemoteEndPoint.ToString(); } }
    }

}