using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Summoner;
using Summoner.Clients;
using Summoner.Notifications;
using Summoner.Util;

namespace Summoner
{
    public class Summoner
    {
        private Object runningLock = new Object();
        private bool running;
        private DateTime? startTime;

        public Summoner(Configuration config)
        {
            Assert.NotNull(config, "config");

            Configuration = config;
        }

        public Configuration Configuration
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
        }

        public DateTime? StartTime
        {
            get
            {
                lock (runningLock)
                {
                    return startTime;
                }
            }
        }

        public void Start()
        {
            Assert.IsTrue(!Running, "!Running");

            List<Client> clients = new List<Client>();
            List<Notification> notifications = new List<Notification>();

            foreach (Dictionary<string, string> clientConfig in Configuration.Clients)
            {
                try
                {
                    Client client = ClientFactory.NewClient(clientConfig);
                    client.Connect();
                    clients.Add(client);
                }
                catch (UnknownClientTypeException e)
                {
                    Console.Error.WriteLine(e.Message);
                    Environment.Exit(1);
                }
            }

            foreach (Dictionary<string, string> notificationConfig in Configuration.Notifications)
            {
                try
                {
                    notifications.Add(NotificationFactory.NewNotification(notificationConfig));
                }
                catch(UnknownNotificationTypeException e)
                {
                    Console.Error.WriteLine(e.Message);
                    Environment.Exit(1);
                }
            }

            lock (runningLock)
            {
                this.running = true;
                this.startTime = DateTime.Now;
            }

            while (Running)
            {
                foreach (Client client in clients)
                {
                    IEnumerable<Message> messages;

                    try
                    {
                        messages = client.RecentMessages();
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(
                            String.Format("Could not receive messages from {0}: {1} (ignoring)",
                             client.Configuration["type"], e.Message));
                        continue;
                    }

                    foreach (Message message in messages)
                    {
                        foreach (Notification notification in notifications)
                        {
                            if (notification.Configuration != null &&
                                notification.Configuration.ContainsKey("contains") &&
                                !message.Content.Contains(notification.Configuration["contains"]))
                            {
                                continue;
                            }

                            try
                            {
                                notification.Notify(client, message);
                            }
                            catch (Exception e)
                            {
                                Console.Error.WriteLine(
                                    String.Format("Could not notify using {0}: {1} (ignoring)",
                                    notification.Configuration["type"], e.Message));
                                continue;
                            }
                        }
                    }
                }

                Thread.Sleep(Configuration.PollInterval * 1000);
            }

            lock (runningLock)
            {
                running = false;
                startTime = null;
            }
        }

        public void Stop()
        {
            running = false;
        }
    }
}
