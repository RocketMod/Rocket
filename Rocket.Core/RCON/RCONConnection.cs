using System.Net.Sockets;
using System.Linq;

namespace Rocket.Core.RCON
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
                    RCONServer.Send(Client, command);
                else
                    RCONServer.Send(Client, command + (!command.Contains('\n') ? "\r\n" : ""));
                return;
            }
        }

        public string Read()
        {
            return RCONServer.Read(Client);
        }

        public void Close()
        {
            this.Client.Close();
            return;
        }

        public string Address { get { return this.Client.Client.RemoteEndPoint.ToString(); } }
    }

}