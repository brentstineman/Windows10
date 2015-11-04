// TAG: #notificationhubjs
//
// This controller exposes two API methods to be used for registering applications with the Azure Notification Hub. 
// An server side API is being used so that the client app does not need to contain credentials for accessing the notification hub
// directly, as this would present a security risk. Ideally, in normal production scenarios, the APIs would be protected and require 
// some type of authentication before they can be called. But this has not been implemented in this sample to keep things as straight-forward 
// as possible. 
// 
// API methods include:
// api/register (POST): given a channel URI, this returns a device ID from the registry
// api/register (PUT): given the registration detail, it creates a notification hub device registry or updates it if on already exists

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;
using WebAPI.Models;
using System.Threading.Tasks;
using System.Web;

namespace WebAPI.Controllers
{
    public class RegisterController : ApiController
    {
        private NotificationHubClient hub;

        public RegisterController()
        {
            hub = Notifications.Instance.Hub;
        }

        /// <summary>
        /// This is the request payload used by the [POST]api/register method
        /// </summary>
        public class regGetRegistrationId
        {
            public string channeluri { get; set; }
        }

        /// <summary>
        /// This is the request payload used by the [PUT]api/register method
        /// </summary>
        public class DeviceRegistration
        {
            public string deviceid { get; set; } // the device ID
            public string platform { get; set; } // what platform, WNS, APNS, etc... 
            public string handle { get; set; } // callback handle for the associated notification service
            public string[] tags { get; set; } // tags to be used in targetting notifications
        }

        /// <summary>
        ///     This creates a registration id based on the provided channel URI
        /// </summary>
        /// <param name="request">the channelURI used by the application</param>
        /// <returns>a notification hub device ID</returns>
        [HttpPost]
        [Route("api/register/")]
        public async Task<HttpResponseMessage> Post([FromBody]regGetRegistrationId request)
        {
            string newRegistrationId = null;
            string channelUri = request.channeluri;

            //todo: validate the URI is for notify.windows.com domain

            // make sure there are no existing registrations for the channel URI provided
            if (channelUri != null)
            {
                var registrations = await hub.GetRegistrationsByChannelAsync(channelUri, 100);

                foreach (RegistrationDescription registration in registrations)
                {
                    if (newRegistrationId == null)
                    {
                        newRegistrationId = registration.RegistrationId;
                    }
                    else
                    {
                        await hub.DeleteRegistrationAsync(registration);
                    }
                }
            }

            if (newRegistrationId == null)
                newRegistrationId = await hub.CreateRegistrationIdAsync();

            return Request.CreateResponse(HttpStatusCode.OK, newRegistrationId);
        }

        /// <summary>
        ///     This creates or updates a registration (with provided channelURI) at the specified id
        /// </summary>
        /// <param name="deviceUpdate">contains the parameters needed to perform the registration update</param>
        /// <returns>nothing</returns>
        [HttpPut]
        [Route("api/register/")]
        public async Task<HttpResponseMessage> Put([FromBody]DeviceRegistration deviceUpdate)
        {
            RegistrationDescription registration = null;
            switch (deviceUpdate.platform.ToLower())
            {
                case "mpns":
                    registration = new MpnsRegistrationDescription(deviceUpdate.handle);
                    break;
                case "wns":
                    registration = new WindowsRegistrationDescription(deviceUpdate.handle);
                    break;
                case "apns":
                    registration = new AppleRegistrationDescription(deviceUpdate.handle);
                    break;
                case "gcm":
                    registration = new GcmRegistrationDescription(deviceUpdate.handle);
                    break;
                default:
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            registration.RegistrationId = deviceUpdate.deviceid;
            registration.Tags = new HashSet<string>(deviceUpdate.tags);
            // optionally you could supplement/override the list of client supplied tags here
            // tags will help you target specific devices or groups of devices depending on your needs

            try
            {
                await hub.CreateOrUpdateRegistrationAsync(registration);
            }
            catch (MessagingException e)
            {
                //ReturnGoneIfHubResponseIsGone(e);
            }

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
