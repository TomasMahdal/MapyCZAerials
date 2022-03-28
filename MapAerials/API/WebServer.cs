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
    /// WebServer inspired by https://gist.github.com/define-private-public/d05bc52dd0bed1c4699d49e2737e80e7
    /// </summary>
    class WebServer
    {
        public string Url { get; private set; }
        public bool ServerIsRunning { get; private set; }

        private HttpListener httpListener;
        private int port = 5050;
        private string urlStructure = "http://localhost:{0}/";

        /// <summary>
        /// init and start WebServer
        /// </summary>
        public void Start()
        {
            // create URL
            Url = string.Format(urlStructure, port);

            // attach HttpListener to URL and start listening
            httpListener = new HttpListener();
            httpListener.Prefixes.Add(Url);

            httpListener.Start();
            Console.WriteLine(Url);
            ServerIsRunning = true;

            // Handle requests
            Task listenTask = HandleRequest();
            listenTask.GetAwaiter().GetResult();
        }

        /// <summary>
        /// stop WebServer
        /// </summary>
        public void Stop()
        {
            // stop server
            ServerIsRunning = false;

            // stop listener
            httpListener.Close();
        }

        public async Task HandleRequest()
        {
            // loop until is WebServer stopped
            while (ServerIsRunning)
            {
                // get info about connection
                HttpListenerContext ctx = await httpListener.GetContextAsync();

                // get request and response
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                Console.WriteLine(req.Url.ToString());

                // Write the response info
                byte[] data = Encoding.UTF8.GetBytes("Test připojení");
                resp.ContentType = "text/html";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }
    }
}
