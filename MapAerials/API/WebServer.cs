﻿using MapAerials.Structures;
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
using System.Windows;

namespace MapAerials.API
{
    /// <summary>
    /// local WebServer for transfering images into simulator
    /// </summary>
    public class WebServer
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
        private MainViewModel viewModel;
        private bool serverThreadActive = false;

        private const string urlStructure = "http://{0}:{1}/getAerials/~z/~x/~y/";
        private const string urlStructureAdvanced = "http://{0}:{1}/getLotusAerials/{2}/~z/~x/~y/";

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
            //currentIP = GetLocalIP();

            // use localhost address instead of IPv4 of network card
            currentIP = IPAddress.Parse("127.0.0.1");

            // create URL
            URL = string.Format(urlStructure, currentIP, port);

            // create special URLs for every type of map
            // this URLs can be used in LOTUS, which can use multiple links for different map types
            foreach(var mapType in viewModel.SupportedMapTypes)
            {
                string url = string.Format(urlStructureAdvanced, currentIP, port, mapType.ID);
                viewModel.LOTUSLinks.Add(new LotusLink(url, mapType.VisibleName));
            }

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
                                // aerials link must contains params
                                if (splittedUrl.Length < 7)
                                {
                                    MessageBox.Show(Languages.GetString("dialog_corruptedLink"), "Web request", MessageBoxButton.OK, MessageBoxImage.Error);
                                    break;
                                }

                                // get image from MapyCZ
                                Bitmap aerials = MapyCZ.getAerials(splittedUrl[3], splittedUrl[4], splittedUrl[2], viewModel.GenericSelectedMapType);
                                
                                // send it to browser
                                SendImageToBrowser(socket, aerials);
                                break;

                            // Lotus aerials
                            case "getLotusAerials":
                                // aerials link must contains params
                                if (splittedUrl.Length < 8)
                                {
                                    MessageBox.Show(Languages.GetString("dialog_corruptedLink"), "Web request", MessageBoxButton.OK, MessageBoxImage.Error);
                                    break;
                                }

                                // get image from MapyCZ
                                MapType selectedType = viewModel.GetMapTypeById(splittedUrl[2]);
                                Bitmap lAerials = MapyCZ.getAerials(splittedUrl[4], splittedUrl[5], splittedUrl[3], selectedType);

                                // send it to browser
                                SendImageToBrowser(socket, lAerials);
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
                // get page content from resources
                string pageContent = reader.ReadToEnd();

                // replace some of keys on page
                pageContent = pageContent.Replace("{%link%}", URL);

                // send page to browser
                SendToBrowser(pageContent, objSocket);
            }
        }

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
    }
}
