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

        public class regGetRegistrationId
        {
            public string channeluri { get; set; }
        }

        public class DeviceRegistration
        {
            public string deviceid { get; set; }
            public string platform { get; set; }
            public string handle { get; set; }
            public string[] tags { get; set; }
        }

        // POST api/register
        // This creates a registration id based on the provided channel URI
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

        // PUT api/register
        // This creates or updates a registration (with provided channelURI) at the specified id
        [HttpPut]
        [Route("api/register/")]
        public async Task<HttpResponseMessage> Put([FromBody]DeviceRegistration deviceUpdate)
        {
            //DeviceRegistration deviceUpdate = null;
            RegistrationDescription registration = null;
            switch (deviceUpdate.platform)
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
