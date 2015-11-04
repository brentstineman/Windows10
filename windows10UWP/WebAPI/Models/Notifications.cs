// TAG: #notificationhubjs	
// This class sets up the Notification Hub client to be used when calls are made to the API (/Controllers/RegisterController.cs)
// It retrieves two property settings that contain the connection string and notification hub name to be used by the API

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