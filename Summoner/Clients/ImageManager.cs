using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Summoner.Util;

namespace Summoner.Clients
{
    public class ImageManager
    {
        private const int sleepTime = 1000;

        private Object runningLock = new Object();
        private bool running;

        private List<QueuedRequest> requestQueue = new List<QueuedRequest>();

        private class QueuedRequest
        {
            public QueuedRequest(string url, string filename)
            {
                Url = url;
                Filename = filename;
            }

            public string Url { get; private set; }
            public string Filename { get; private set; }
        }

        public ImageManager(Client client)
        {
            Client = client;
        }

        public Client Client
        {
            get;
            private set;
        }

        public bool Running
        {
            get
            {
                lock (runningLock)
                {
                    return running;
                }
            }

            private set
            {
                lock (runningLock)
                {
                    running = value;
                }
            }
        }

        public string GetImage(string imageUrl)
        {
            SHA1 sha = new SHA1CryptoServiceProvider();

            string hash = StringUtil.HexString(sha.ComputeHash(Encoding.ASCII.GetBytes(imageUrl)));
            string filename = Path.Combine(Client.Configuration.Global.ImageCacheDir, hash);

            if (!File.Exists(filename))
            {
                Enqueue(new QueuedRequest(imageUrl, filename));
            }

            Thread.Sleep(sleepTime * 2);

            return filename;
        }

        private void Enqueue(QueuedRequest request)
        {
            lock (requestQueue)
            {
                requestQueue.Add(request);
            }
        }

        private QueuedRequest Dequeue()
        {
            QueuedRequest request = null;

            lock (requestQueue)
            {
                if (requestQueue.Count > 0)
                {
                    request = requestQueue[0];
                    requestQueue.RemoveAt(0);
                }
            }

            return request;
        }

        private void HandleRequest(QueuedRequest queuedRequest)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(queuedRequest.Url);

            if (Client.Configuration.ContainsKey("username") && Client.Configuration.ContainsKey("password"))
            {
                request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(
                    Encoding.ASCII.GetBytes(String.Format("{0}:{1}",
                    Client.Configuration["username"], Client.Configuration["password"]))));
            }

            Directory.CreateDirectory(Path.GetDirectoryName(queuedRequest.Filename));

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            using (Stream outputStream = File.Open(queuedRequest.Filename, FileMode.OpenOrCreate, FileAccess.Write))
            {
                byte[] buf = new byte[4096];
                int readlen;

                while ((readlen = responseStream.Read(buf, 0, 4096)) > 0)
                {
                    outputStream.Write(buf, 0, readlen);
                }

                outputStream.Close();
                responseStream.Close();
                response.Close();
            }
        }

        public void Start()
        {
            Running = true;

            while (Running)
            {
                QueuedRequest queuedRequest;

                while ((queuedRequest = Dequeue()) != null)
                {
                    HandleRequest(queuedRequest);
                }

                Thread.Sleep(sleepTime);
            }
        }

        public void Stop()
        {
            Running = false;
        }
    }
}
