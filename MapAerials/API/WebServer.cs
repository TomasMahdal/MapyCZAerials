using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace MapAerials.API
{
    /// <summary>
    /// local WebServer for transfering images into simulator
    /// </summary>
    class WebServer
    {
        public string URL { get; private set; }

        public bool ServerIsRunning {
            get {
                return serverThreadActive && webListener.IsListening;
            }
        }

        private WebListener webListener;
        private Thread webserverCore;
        private int port = 5050;
        private IPAddress currentIP;
        private string urlStructure = "http://{0}:{1}/getAerials/~z/~x/~y/";
        private MainViewModel viewModel;
        private bool serverThreadActive = false;

        public WebServer(MainViewModel _viewModel)
        {
            viewModel = _viewModel;
        }

        /// <summary>
        /// init and start WebServer
        /// </summary>
        public void Start()
        {
            // get IP
            currentIP = GetLocalIP();

            // create URL
            URL = string.Format(urlStructure, currentIP, port);

            // start listening
            webListener = new WebListener(currentIP, port);
            webListener.Start();
            serverThreadActive = true;

            // force update of status
            viewModel.UpdateProperty("ServerStatus");
            viewModel.UpdateProperty("ServerStatusColor");

            // run server core thread
            webserverCore = new Thread(new ThreadStart(WebServerCore));
            webserverCore.Start();
        }

        /// <summary>
        /// stop WebServer
        /// </summary>
        public void Stop()
        {
            serverThreadActive = false;
            webListener.Stop();

            // force update of status
            viewModel.UpdateProperty("ServerStatus");
            viewModel.UpdateProperty("ServerStatusColor");
        }

        public void WebServerCore()
        {
            while (serverThreadActive)
            {
                if (!webListener.Pending())
                {
                    Thread.Sleep(10);
                    continue;
                }

                // accept connection
                Socket socket = webListener.AcceptSocket();

                if (socket.Connected)
                {
                    // get data from client
                    Byte[] bReceive = new Byte[1024];
                    int i = socket.Receive(bReceive, bReceive.Length, 0);

                    // convert data from client to string
                    string sBuffer = Encoding.UTF8.GetString(bReceive);

                    // request must contains HTTP
                    if (sBuffer.Contains("HTTP"))
                    {
                        // get requestedURL
                        string requestedUrl = sBuffer.Substring(0, sBuffer.IndexOf("HTTP", 1));
                        requestedUrl = requestedUrl.Substring(3).Replace(" ", "") + "/";

                        // split by /
                        var splittedUrl = requestedUrl.Split('/');

                        switch (splittedUrl[1])
                        {
                            // homepage
                            case "":
                                SendHTMLFromResources("MapAerials.API.htdocs.index.html", socket);
                                break;

                            // aerial page
                            case "getAerials":
                                // get image from MapyCZ
                                Bitmap aerials = MapyCZ.getAerials(splittedUrl[3], splittedUrl[4], splittedUrl[2], MapyCZ.SupportedMapTypes[0]);
                                
                                // send it to browser
                                SendImageToBrowser(socket, aerials);
                                break;
                            // unknown page
                            default:
                                SendHTMLFromResources("MapAerials.API.htdocs.error404.html", socket);
                                break;
                        }
                    }
                    else
                    {
                        SendHTMLFromResources("MapAerials.API.htdocs.error404.html", socket);
                    }

                    socket.Close();
                }
            }
        }

        /// <summary>
        /// Send data to browser
        /// </summary>
        /// <param name="returnData">HTML string</param>
        /// <param name="socketObj">socket object</param>
        private void SendToBrowser(string returnData, Socket socketObj)
        {
            Byte[] dataForSend = Encoding.UTF8.GetBytes(returnData);
            SendHeader(dataForSend.Length, "200", socketObj);
            if (socketObj.Connected)
            {
                socketObj.Send(dataForSend);
            }
        }

        /// <summary>
        /// Send response header to browser
        /// </summary>
        private void SendHeader(int lenghtOfBytes, string statusCode, Socket socketObj, string contentType = "text/html")
        {
            string sBuffer = "";

            sBuffer = sBuffer + "HTTP/1.1 " + statusCode + "\r\n";
            sBuffer = sBuffer + "Server: LOTUSWebServer\r\n";
            sBuffer = sBuffer + "Content-Type: " + contentType + "\r\n";
            sBuffer = sBuffer + "Connection: close\r\n";
            sBuffer = sBuffer + "Accept-Ranges: bytes\r\n";
            sBuffer = sBuffer + "Content-Length: " + lenghtOfBytes + "\r\n\r\n";

            if (socketObj.Connected)
            {
                socketObj.Send(Encoding.ASCII.GetBytes(sBuffer));
            }
        }

        /// <summary>
        /// Read HTML page from resources and send it to browser
        /// </summary>
        /// <param name="path">path to page</param>
        /// <param name="objSocket">currently used socket</param>
        private void SendHTMLFromResources(string path, Socket objSocket)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (StreamReader reader = new StreamReader(assembly.GetManifestResourceStream(path)))
            {
                SendToBrowser(reader.ReadToEnd(), objSocket);
            }
        }

        /// <summary>
        /// Send PNG image to browser
        /// </summary>
        /// <param name="objSocket">currently used socket</param>
        /// <param name="img">image to send</param>
        public void SendImageToBrowser(Socket objSocket, Image img)
        {
            byte[] imgBytes = (byte[])(new ImageConverter()).ConvertTo(img, typeof(byte[]));

            SendHeader(imgBytes.Length, "200", objSocket, "image/png");

            if (objSocket.Connected)
            {
                objSocket.Send(imgBytes);
            }
        }

        /// <summary>
        /// Get IPv4 address of local network interface
        /// </summary>
        /// <returns>IPv4 of local network interface</returns>
        private static IPAddress GetLocalIP()
        {
            foreach (IPAddress ip in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("No network adapter with IPv4 address found!");
        }
    }
}
