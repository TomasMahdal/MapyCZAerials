using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;

namespace MapAerials
{
    class MainViewModel : INotifyPropertyChanged
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

        public API.WebServer WServer { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Status of server in string
        /// </summary>
        public string ServerStatus
        {
            get
            {
                if (WServer == null)
                {
                    return "offline";
                }

                if (WServer.ServerIsRunning)
                {
                    return "online";
                }
                else
                {
                    return "offline";
                }
            }
        }

        /// <summary>
        /// Server status color
        /// </summary>
        public Brush ServerStatusColor
        {
            get
            {
                if (ServerStatus == "online")
                {
                    return new SolidColorBrush(Color.FromRgb(6, 169, 169));
                }
                else
                {
                    return new SolidColorBrush(Color.FromRgb(180, 0, 0));
                }
            }
        }

        /// <summary>
        /// create and start new WebServer
        /// </summary>
        public void StartServer()
        {
            if (WServer == null)
            {
                WServer = new API.WebServer(this);
                WServer.Start();
            }

            UpdateProperty("WServer");
        }

        /// <summary>
        /// stop currently running WebServer
        /// </summary>
        public void StopServer()
        {
            if (WServer != null)
            {
                WServer.Stop();
                WServer = null;
            }
        }

        public void UpdateProperty(string s)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(s));
        }
    }
}
 