using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MapAerials.API
{
    /// <summary>
    /// local WebServer for transfering images into simulator
    /// </summary>
    class WebServer
    {
        public string Url { get; private set; }
        public bool ServerIsRunning { get; private set; }

        private TcpListener webListener;
        private Thread webserverCore;
        private int port = 5050;
        private IPAddress currentIP;
        private string urlStructure = "http://{0}:{1}/";

        /// <summary>
        /// init and start WebServer
        /// </summary>
        public void Start()
        {
            // get IP
            currentIP = GetLocalIP();

            // create URL
            Url = string.Format(urlStructure, currentIP, port);
            Console.WriteLine(Url);

            // start listening
            webListener = new TcpListener(currentIP, port);
            webListener.Start();
            ServerIsRunning = true;

            // run server core thread
            webserverCore = new Thread(new ThreadStart(WebServerCore));
            webserverCore.Start();
        }

        /// <summary>
        /// stop WebServer
        /// </summary>
        public void Stop()
        {
           
        }

        public void WebServerCore()
        {
            while (ServerIsRunning)
            {
                // accept connection
                Socket socket = webListener.AcceptSocket();

                if (socket.Connected)
                {
                    // get data from client
                    Byte[] bReceive = new Byte[1024];

                    // convert data from client to string
                    string sBuffer = Encoding.UTF8.GetString(bReceive);

                    Console.WriteLine(sBuffer);

                    SendToBrowser("test", socket);
                }
            }
        }

        /// <summary>
        /// Send data to browser
        /// </summary>
        /// <param name="returnData">HTML string</param>
        /// <param name="socketObj">socket object</param>
        public void SendToBrowser(string returnData, Socket socketObj)
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
