using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapAerials
{
    class MainViewModel
    {
        /// <summary>
        /// Supported map types by Mapy.cz API
        /// </summary>
        public List<Structures.MapType> SupportedMapTypes
        {
            get
            {
                return API.MapyCZ.SupportedMapTypes;
            }
        }

        private API.WebServer webServer;

        /// <summary>
        /// create and start new WebServer
        /// </summary>
        public void StartServer()
        {
            if (webServer == null)
            {
                webServer = new API.WebServer();
                webServer.Start();
            }
        }

        /// <summary>
        /// stop currently running WebServer
        /// </summary>
        public void StopServer()
        {
            if (webServer != null)
            {
                webServer.Stop();
                webServer = null;
            }
        }
    }
}
 