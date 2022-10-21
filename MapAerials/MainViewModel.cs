using MapAerials.Structures;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace MapAerials
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private MainWindow parent;

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

        /// <summary>
        /// Map type selected by user
        /// </summary>
        public Structures.MapType GenericSelectedMapType { get; set; }

        /// <summary>
        /// Links for LOTUS simulator
        /// </summary>
        public List<LotusLink> LOTUSLinks { get; set; }

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
                    return new SolidColorBrush(Color.FromRgb(180, 20, 20));
                }
            }
        }

        private NotificationIcon notificationIncon;

        public MainViewModel(MainWindow mainWindow)
        {
            parent = mainWindow;
            notificationIncon = new NotificationIcon(this);
            LOTUSLinks = new List<LotusLink>();
        }

        /// <summary>
        /// Create and start new WebServer
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
        /// Stop currently running WebServer
        /// </summary>
        public void StopServer()
        {
            if (WServer != null)
            {
                WServer.Stop();
                WServer = null;
            }

            UpdateProperty("WServer");
        }

        public void UpdateProperty(string s)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(s));
        }

        /// <summary>
        /// Copy URL to clipboard
        /// </summary>
        public void CopyURL()
        {
            if (WServer != null)
            {
                Clipboard.SetText(WServer.URL);
            }
        }

        /// <summary>
        /// Make MainForm visible again
        /// </summary>
        public void ShowMainForm(object sender, EventArgs e)
        {
            parent.Show();
        }

        /// <summary>
        /// Force all threads to stop and exit app
        /// </summary>
        public void ExitApp(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        /// <summary>
        /// Get map type from supported (default = airplane type)
        /// </summary>
        /// <param name="id">ID of map</param>
        /// <returns>type of map</returns>
        public MapType GetMapTypeById(string id)
        {
            int mapTypeId = 0;
            int.TryParse(id, out mapTypeId);

            foreach(MapType type in SupportedMapTypes)
            {
                if (type.ID == mapTypeId)
                {
                    return type;
                }
            }

            // if not found, return first map type
            return SupportedMapTypes.First();
        }
    }
}
 