
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
                    // get a Notification Hub registration ID
                    return new WinJS.xhr({
                        type: "POST",
                        url: "http://localhost:7521/api/register",
                        headers: {
                            "Content-Type": "application/json"
                        },
                        data: { "channeluri": "mystring" }
                    }).then(function (req) {
                        location = req.getResponseHeader("Content-Location");

                        // do something with the result

                        // save/update channel URI
                        localSettings.values["WNSChannelURI"] = currentChannelURI;
                    })
                }
            }).done();
        }
    };
// application/json