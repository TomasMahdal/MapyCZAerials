using System;
using System.Collections.Generic;
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
        public bool ServerIsRunning { get; private set; }

        private TcpListener webListener;
        private Thread webserverCore;
        private int port = 5050;
        private IPAddress currentIP;
        private string urlStructure = "http://{0}:{1}/";
        private MainViewModel viewModel;

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
            webListener = new TcpListener(currentIP, port);
            webListener.Start();
            ServerIsRunning = true;

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
            ServerIsRunning = false;
            webListener.Stop();

            // force update of status
            viewModel.UpdateProperty("ServerStatus");
            viewModel.UpdateProperty("ServerStatusColor");
        }

        public void WebServerCore()
        {
            while (ServerIsRunning)
            {
                if (!webListener.Pending())
                {
                    Thread.Sleep(500);
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
                        requestedUrl = requestedUrl.Substring(3).Replace(" ", "");

                        SendHTMLFromResources("MapAerials.API.htdocs.index.html", socket);
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
