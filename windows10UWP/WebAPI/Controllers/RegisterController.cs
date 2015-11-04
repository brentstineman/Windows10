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
            public string Platform { get; set; }
            public string Handle { get; set; }
            public string[] Tags { get; set; }
        }

        //[HttpGet]
        //[Route("api/registrations")]
        //public async Task<HttpResponseMessage> Get()
        //{
        //    //var registrationsCount = await hub.GetAllRegistrationsAsync(0);
        //    //var registrationsCount = await hub.GetRegistrationsByTagAsync("tag1",10);

        //    string connectionString = @"Endpoint=sb://bmstesthub.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=D+fvSAjEMbA2dBHeX3J181Y5oTAo54pYId3UCHQAxUc=";
        //    string hubName = "bmstesthub";

        //    var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);

        //    NotificationHubClient hubClient = NotificationHubClient.CreateClientFromConnectionString(connectionString, hubName);
        //    var registrations = hubClient.GetRegistrationsByTagAsync("tag1", 1).Result;

        //    return Request.CreateResponse(HttpStatusCode.OK, registrations.Count() + " devices registered");
        //}

        [HttpPut]
        [Route("api/register/{stuff}")]
        //public async Task<HttpResponseMessage> Put(string stuff)
        //{
        //    return Request.CreateErrorResponse(HttpStatusCode.OK, "Simple Put worked");
        //}

        // POST api/register
        // This creates a registration id based on the provided channel URI
        [HttpPost]
        [Route("api/register/")]
        public async Task<HttpResponseMessage> Post([FromBody]regGetRegistrationId request)
        {
            string newRegistrationId = null;
            string channelUri = request.channeluri;

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
        [Route("api/register/{id}")]
        public async Task<HttpResponseMessage> Put(string id, [FromBody]DeviceRegistration deviceUpdate)
        {
            //DeviceRegistration deviceUpdate = null;
            RegistrationDescription registration = null;
            switch (deviceUpdate.Platform)
            {
                case "mpns":
                    registration = new MpnsRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "wns":
                    registration = new WindowsRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "apns":
                    registration = new AppleRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "gcm":
                    registration = new GcmRegistrationDescription(deviceUpdate.Handle);
                    break;
                default:
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            registration.RegistrationId = id;
            registration.Tags = new HashSet<string>(deviceUpdate.Tags);

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

        //// DELETE api/register/5
        //public async Task<HttpResponseMessage> Delete(string id)
        //{
        //    await hub.DeleteRegistrationAsync(id);
        //    return Request.CreateResponse(HttpStatusCode.OK);
        //}

        //private static void ReturnGoneIfHubResponseIsGone(MessagingException e)
        //{
        //    var webex = e.InnerException as WebException;
        //    if (webex.Status == WebExceptionStatus.ProtocolError)
        //    {
        //        var response = (HttpWebResponse)webex.Response;
        //        if (response.StatusCode == HttpStatusCode.Gone)
        //            throw new HttpRequestException(HttpStatusCode.Gone.ToString());
        //    }
        //}
    }
}
