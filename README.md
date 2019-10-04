# GoogleAnalyticsClientDotNet

This project provides basic API integration for using Google Analytics track event from .NET
applications.  

Supported Platform : UWP, .NET(4.5.1)

### Current Features:

* Make basic calls to https://developers.google.com/analytics/devguides/collection/protocol/v1/ and track events

### Usage:

Step 1. install nuget:
* [UWP](https://www.nuget.org/packages/GoogleAnalyticsClientDotNet.Universal/)
* [WPF](https://www.nuget.org/packages/GoogleAnalyticsClientDotNet.Net45/)
* [.Net Standard](https://www.nuget.org/packages/GoogleAnalyticsClientDotNet.Standard/)

Step 2. new the AnalyticsService instance.

```csharp
AnalyticsService service = new AnalyticsService();
service.Initialize("{tracking id}", "{appName}", "{appId}", "{appVersion}");

// If you install .NET Standard version, must setting DefaultUserAgent property
service.DefaultUserAgent = "{default user agent}";
```

Step 3. new the EventParameter, and set properties.

```csharp
EventParameter eventData = new EventParameter();
eventData.Category = "";
eventData.Action = "";
eventData.Label = "";
eventData.ScreenName = "";
eventData.ClientId = "";
eventData.UserAgent = deviceService.ModelName;
            
service.TrackEvent(eventData);
```

Step 4. For WPF or .NET Standard, need call method: SaveTempEventsData() to keep not upload events.
```csharp
private async void MainWindow_Closed(object sender, EventArgs e)
{
    await service?.SaveTempEventsData();
}
```

Step 5. For Universal Windows Platform. User need to add SaveTempEventsData method in the Suspened event.
```csharp
/// <summary>
/// Invoked when application execution is being suspended.  Application state is saved
/// without knowing whether the application will be terminated or resumed with the contents
/// of memory still intact.
/// </summary>
/// <param name="sender">The source of the suspend request.</param>
/// <param name="e">Details about the suspend request.</param>
private async void OnSuspending(object sender, SuspendingEventArgs e)
{
    var deferral = e.SuspendingOperation.GetDeferral();

    await service.SaveTempEventsData();

    deferral.Complete();
}
```

### Licence

[Licenced under the Apache 2.0 licence](https://github.com/poumason/GoogleAnalyticsClientDotNet/blob/master/license.txt)

### Reference
[Google Analytics SDK for Windows and Windows Phone](https://googleanalyticssdk.codeplex.com/)