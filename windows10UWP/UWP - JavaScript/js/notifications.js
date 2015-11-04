
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
                    console.log("Channel URI has changed, need to register it with notification hub");

                    // get a Notification Hub registration ID
                    WinJS.xhr({
                        type: "post",
                        url: "http://localhost:7521/api/register",
                        headers: { "Content-type": "application/x-www-form-urlencoded" },
                        responseType: "text",
                        data: "channeluri=" + currentChannelURI.toLowerCase()
                    }).then(function (getIdSuccess) {
                            // strip the double quotes off the string, we don't want those
                        var deviceId = getIdSuccess.responseText.replace(/['"]/g, '');
                            console.log("Device ID is: " + deviceId);

                            // create object for notification hub device registration
                            var registrationpayload = {
                                "deviceid"  : deviceId,
                                "platform"  : "wns",
                                "handle"    : currentChannelURI,
                                "tags"      : ["tag1", "tag2"]
                            };

                            // update the registration
                            WinJS.xhr({
                                type: "put",
                                url: "http://localhost:7521/api/register/",
                                headers: { "Content-type": "application/json" },
                                data: JSON.stringify(registrationpayload)
                            }).then(
                                function (registerSuccess) {
                                    console.log("Device successfully registered");

                                    // save/update channel URI for next app launch
                                    localSettings.values["WNSChannelURI"] = currentChannelURI;
                                },
                                function (error) {
                                    console.log(JSON.parse(error.responseText));
                                }
                            ).done();
                        },
                        function (error) {
                            console.log(JSON.parse(error.responseText));
                        }
                    ).done();
                }
            }).done();
        }
    };