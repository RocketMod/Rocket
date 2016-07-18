using System;
using System.Net;

namespace Rocket.API
{
    public class RocketWebClient : WebClient
    {
        public int Timeout { get; set; }

        public RocketWebClient()  : this(30000)
        {
        }

        public RocketWebClient(int timeout)
        {
            this.Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = this.Timeout;
            }
            return request;
        }
    }
}