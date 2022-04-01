using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MapAerials.API
{
    /// <summary>
    /// Extension of TcpListener for WebServer
    /// </summary>
    public class WebListener : TcpListener
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="localEP">IP adress on which listen</param>
        public WebListener(IPEndPoint localEP) : base(localEP)
        {
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="localaddr">IP adress on which listen</param>
        /// <param name="port">port on which to listen</param>
        public WebListener(IPAddress localaddr, int port) : base(localaddr, port)
        {
        }

        /// <summary>
        /// Status, that WebListener can listen sockets
        /// </summary>
        public bool IsListening
        {
            get { return base.Active; }
        }
    }
}
