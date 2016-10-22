# GoogleAnalyticsClientDotNet

This project provides basic API integration for using Google Analytics track event from .NET
applications.  

Supported Platform : UWP, .NET(4.5.1)

### Current Features:

* Make basic calls to https://developers.google.com/analytics/devguides/collection/protocol/v1/ and track events

### Usage:

Step 1. install nuget:
* UWP: https://www.nuget.org/packages/GoogleAnalyticsClientDotNet.Universal/
* WPF: https://www.nuget.org/packages/GoogleAnalyticsClientDotNet.Net45/

Step 2. new the MixpanelClient instance.

```csharp
AnalyticsService tracker = new AnalyticsService();
service.Initialize("{tracking id}");
```

Step 3. new the EventParameter, and set properties.

```csharp
EventParameter eventData = new EventParameter();
eventData.Category = "";
eventData.Action = "";
eventData.Label = " Playlist";
eventData.ScreenName = "";
eventData.ClientId = "";
eventData.UserAgent = deviceService.ModelName;
            
service.TrackEvent(eventData);
```

### Licence

[Licenced under the Apache 2.0 licence](https://github.com/poumason/GoogleAnalyticsClientDotNet/blob/master/license.txt)