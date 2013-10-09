using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Summoner.Util;

namespace Summoner.Notifications
{
    public class NotificationFactory
    {
        public static Notification NewNotification(Dictionary<string, string> configuration)
        {
            Assert.NotNull(configuration, "configuration");

            if (configuration["type"].Equals("console"))
            {
                return new ConsoleNotification(configuration);
            }
            else if (configuration["type"].Equals("growl"))
            {
                return new GrowlNotification(configuration);
            }
            else if (configuration["type"].Equals("sms"))
            {
                return new SmsNotification(configuration);
            }

            throw new UnknownNotificationTypeException(
                String.Format("Unknown notification type: {0}", configuration["type"]));
        }
    }

    public class UnknownNotificationTypeException : Exception
    {
        public UnknownNotificationTypeException(string message)
            : base(message)
        {
        }
    }
}
