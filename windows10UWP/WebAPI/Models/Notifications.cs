using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Microsoft.Azure.NotificationHubs;

namespace WebAPI.Models
{
    public class Notifications
    {
        public static Notifications Instance = new Notifications();

        public NotificationHubClient Hub { get; set; }

        private Notifications()
        {
            //get properties that have NH connection info
            string nhConnString = Properties.Settings.Default.NotificationHubConnectionString;
            string nhName = Properties.Settings.Default.NotificationHubName;

            // create a NH client 
            Hub = NotificationHubClient.CreateClientFromConnectionString(nhConnString, nhName);
        }
    }
}