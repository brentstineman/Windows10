// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
	"use strict";

	var app = WinJS.Application;
	var activation = Windows.ApplicationModel.Activation;

	app.onactivated = function (args) {
		if (args.detail.kind === activation.ActivationKind.launch) {
			if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
				// TODO: This application has been newly launched. Initialize your application here.
			} else {
				// TODO: This application was suspended and then terminated.
				// To create a smooth user experience, restore application state here so that it looks like the app never stopped running.
			}
			args.setPromise(WinJS.UI.processAll());
		}
	};

    // check and see if we have a saved ChannelURI
    var applicationData = Windows.Storage.ApplicationData.current;
    var localSettings = applicationData.localSettings;

    var savedChannelURI = localSettings.values["WNSChannelURI"];

    // get current channel URI for notifications
    var pushNotifications = Windows.Networking.PushNotifications;
    var channelOperation = pushNotifications.PushNotificationChannelManager.createPushNotificationChannelForApplicationAsync();

    // get current channel URI and check against saved URI
    channelOperation.then(function (newChannel) {
        return newChannel.uri;
    }).then(function (currentChannelURI) {
        // if we don't have a saved URI, or its changed, re-register with Notification Hub 
        if (!savedChannelURI || savedChannelURI.toLowerCase() != currentChannelURI.toLowerCase()) {
            // register with notification hub
            //TODO: call API to register with notification hub

            // save/update channel URI
            localSettings.values["WNSChannelURI"] = currentChannelURI;
        }
    }).done();

	app.oncheckpoint = function (args) {
		// TODO: This application is about to be suspended. Save any state that needs to persist across suspensions here.
		// You might use the WinJS.Application.sessionState object, which is automatically saved and restored across suspension.
		// If you need to complete an asynchronous operation before your application is suspended, call args.setPromise().
	};

	app.start();
})();
