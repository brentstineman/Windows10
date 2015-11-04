# Windows 10 Samples
This project is dedicated to helping folks find easy to re-use code samples for building Windows 10 Universal Windows Apps. Usage is simple, find a scenario below, and use its tag to locate where in the source code of this solution the various bits and pieces reside. Some scenarios also provide links to write-ups that go into more detail on the scenario and provide background links. 

Its hoped that over time, the scenarios shown here will expand. 

**Using Notification Hub with a HTML5/JavaScript Application**
tags: #notificationhubjs

Use Azure Notification Hub to send push notifications to a Windows 10 UWP. This sample uses JavaScript code during application startup to call a c# based API to register the device for notifications. While the app could self register, this would require the notification hub credentials to be embedded in the application which presents a security risk. 

To leverage this sample you will need to:


1. go to dev.windows.com and register an application for development
2. configure the application for Windows Push Notification Services (WPNS) and capture the Package SID, and Client secret
3. create an Azure notification hub, capture the connection string and hub name
4. connect the hub to The WPNS using the Package SID and Client Secret
5. create properties for **NotificationHubConnectionString** and **NotificationHubName** in the WebAPI project and use those to store the values you captured in step 3
6. Associate the "UWP - JavaScript" project with the store app you registered in step 1

If these changes are in place properly, you can launch first the WebAPI project (which runs the c# api used to register for notification hub), then the UWP application. When the UWP app starts up, it will attempt to call the API to register for notifications. To test the app, return to the Notification Hub area of the Azure Portal and use its "debug" option to send a test notification to the application. 

For additional details see [this blog post](https://brentdacodemonkey.wordpress.com/). 
