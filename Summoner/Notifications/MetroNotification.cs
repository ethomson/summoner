using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.IO;
using System.Drawing;

using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

using Summoner;
using Summoner.Clients;
using Summoner.Util;

namespace Summoner.Notifications
{
    public class MetroNotification : Notification
    {
        public MetroNotification(ConfigurationDictionary configuration)
        {
            this.Configuration = configuration;

            if (configuration.IsTrue("debuginstall"))
            {
                DebugInstall();
            }
        }

        public ConfigurationDictionary Configuration
        {
            get;
            private set;
        }

        private void DebugInstall()
        {
            string defaultPath = string.Format(@"{0}\Microsoft\Windows\Start Menu\Programs\{1}.Debug.lnk",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                Constants.ApplicationName);

            if (File.Exists(defaultPath))
            {
                return;
            }

            string fullPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            using (ShellLink shellLink = new ShellLink())
            {
                shellLink.Path = fullPath;
                shellLink.Arguments = "";
                shellLink.AppUserModelId = Constants.ApplicationId;
                shellLink.Commit();
                shellLink.SaveTo(defaultPath);
            }
        }

        private string GetDefaultSenderImage()
        {
            string filename = Path.Combine(Configuration.Global.ImageCacheDir, "default.gif");

            if (!File.Exists(filename))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filename));
                    Resources.Instance.ExtractResourceToFile("PersonPlaceholder", filename);
                }
                catch(Exception)
                {
                    // TODO: log...
                }
            }

            return filename;
        }

        public void Notify(Client client, Message message)
        {
            ToastTemplateType toastTemplate = ToastTemplateType.ToastImageAndText02;
            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);

            XmlNodeList toastTextAttributes = toastXml.GetElementsByTagName("text");
            toastTextAttributes[0].InnerText = message.Sender;
            toastTextAttributes[1].InnerText = message.Content;

            XmlNodeList toastImageAttributes = toastXml.GetElementsByTagName("image");

            if (message.SenderImage != null && File.Exists(message.SenderImage))
            {
                ((XmlElement)toastImageAttributes[0]).SetAttribute("src", message.SenderImage);
            }
            else
            {
                ((XmlElement)toastImageAttributes[0]).SetAttribute("src", GetDefaultSenderImage());
            }

            ((XmlElement)toastImageAttributes[0]).SetAttribute("alt", message.Sender);

            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");

            XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.IM");

            if (Configuration.IsFalse("audio"))
            {
                audio.SetAttribute("silent", "true");
            }

            toastNode.AppendChild(audio);


            ToastNotification toast = new ToastNotification(toastXml);

            toast.Activated += toast_Activated;
            toast.Dismissed += toast_Dismissed;
            toast.Failed += toast_Failed;

            ToastNotificationManager.CreateToastNotifier(Constants.ApplicationId).Show(toast);
        }

        void toast_Activated(ToastNotification sender, object args)
        {
        }

        void toast_Dismissed(ToastNotification sender, ToastDismissedEventArgs args)
        {
        }

        void toast_Failed(ToastNotification sender, ToastFailedEventArgs args)
        {
            Console.Error.WriteLine("Win8 Toast Notification Failed: {0}", args.ErrorCode.Message);
        }
    }
}
