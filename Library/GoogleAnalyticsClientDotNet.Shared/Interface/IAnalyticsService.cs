﻿using GoogleAnalyticsClientDotNet.ServiceModel;
using System;
using System.Threading.Tasks;

namespace GoogleAnalyticsClientDotNet
{
    public interface IAnalyticsService : IDisposable
    {
        void Initialize(string trackingId, string appName, string appId, string appVersion);

        void Initialize(string trackingId, string appName, string appId, string appVersion, ILocalTracker localTracker);

        string TrackingID { get; set; }

        void TrackEvent(string categroy, string action, string label, string value, string screenName);

        void TrackEvent(BaseMeasurementParameter eventItem);

        Task SaveTempEventsData(bool replace = false);
    }
}