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

			    // TAG: #notificationhubjs			    
			    uwpNotifications.registerChannelURI(); // register the app for notifications
			} else {
				// TODO: This application was suspended and then terminated.
				// To create a smooth user experience, restore application state here so that it looks like the app never stopped running.
			}
			args.setPromise(WinJS.UI.processAll());
		}

	    // Anything where you need a DOM needs to be done after the onactivated has been called - prior to that the DOM is not loaded
	    // TAG: #draganddropjs
		var holder = document.getElementById('holder');
		DragAndDrop.setup(document);

	};

	app.oncheckpoint = function (args) {
		// TODO: This application is about to be suspended. Save any state that needs to persist across suspensions here.
		// You might use the WinJS.Application.sessionState object, which is automatically saved and restored across suspension.
		// If you need to complete an asynchronous operation before your application is suspended, call args.setPromise().
	};

    // THIS kicks off the app ful load and run - init the DOM, etc.  After the onactivated is then called.
	app.start();

})();
