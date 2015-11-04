
    // this object encapsulates the functions related to 
    // registering for and processing notifications sent via 
    // WNS (Windows Notification Service)
    var uwpNotifications = {
        // this function gets the channel URI, compares it to a cached version
        // and if it has changed will call an API to re-register the application 
        // to recieve notifications
        registerChannelURI: function(){
            // check and see if we have a saved ChannelURI
            var applicationData = Windows.Storage.ApplicationData.current;
            var localSettings = applicationData.localSettings;

            //var savedChannelURI = localSettings.values["WNSChannelURI"];
            var savedChannelURI = "bypass";

            // get current channel URI for notifications
            var pushNotifications = Windows.Networking.PushNotifications;
            var channelOperation = pushNotifications.PushNotificationChannelManager.createPushNotificationChannelForApplicationAsync();

            // get current channel URI and check against saved URI
            channelOperation.then(function (newChannel) {
                return newChannel.uri;
            }).then(function (currentChannelURI) {
                // if we don't have a saved URI, or its changed, re-register with Notification Hub 
                if (!savedChannelURI || savedChannelURI.toLowerCase() != currentChannelURI.toLowerCase()) {
                    //// get a Notification Hub registration ID

                    WinJS.xhr({
                        type: "post",
                        url: "http://localhost:7521/api/register",
                        headers: { "Content-type": "application/x-www-form-urlencoded" },
                        responseType: "text",
                        data: "channeluri=" + currentChannelURI.toLowerCase()
                    }).then(
                        function (success) {
                            // strip the double quotes off the string, we don't want those
                            var deviceId = success.responseText.replace(/['"]/g,'');
                            console.log("Device ID is: " + deviceId);

                            // update the registration

                            // save/update channel URI
                            localSettings.values["WNSChannelURI"] = currentChannelURI;
                        },
                        function (error) {
                            console.log(JSON.parse(error.responseText));
                        }
                    ).done();

                    //    // do something with the result
                    //    var registrationpayload = {
                    //        "platform": "wns",
                    //        "handle": location,
                    //        "tags": ["tag1", "tag2"]
                    //    };
                }
            }).done();
        }
    };
// application/json