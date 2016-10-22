# GoogleAnalyticsClientDotNet

This project provides basic API integration for using Google Analytics track event from .NET
applications.  

Supported Platform : UWP, .NET(4.5.1)

### Current Features:

* Make basic calls to http://api.mixpanel.com/track and track events

### Usage:

Step 1. install nuget:
* UWP: https://www.nuget.org/packages/MixpanelDotNet.UWP/
* WPF: https://www.nuget.org/packages/MixpanelClientDotNet.WPF

Step 2. new the MixpanelClient instance.

```csharp
IMixpanelClient tracker = new MixpanelClient("your API token");
```

Step 3. new the EventData, and set properties.

```csharp
var eventData = new EventData("event name");
eventData.SetProperty("key1", 1);
eventData.SetProperty("key2", "name");
eventData.SetProperty("ket3", true);
tracker.TrackEvent(eventData);
```

### Licence

[Licenced under the Apache 2.0 licence](https://github.com/poumason/Mixpanel.Net.Client/blob/master/license.txt)