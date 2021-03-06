﻿using GoogleAnalyticsClientDotNet.Utility;

namespace GoogleAnalyticsClientDotNet
{
    public class AnalyticsService : BaseAnalyticsService
    {

        protected override void InitializeProcess()
        {
            NetworkTool = new NetworkHelper();

            if (LocalTracker == null)
            {
                LocalTracker = new FileLocalTracker();
            }

            BuildUserAgent();
        }

        protected override void BuildUserAgent()
        {
            DeviceInformationService deviceService = new DeviceInformationService();
            DefaultUserAgent = $"Mozilla/5.0 (compatible; MSIE {deviceService?.IEVersion}.0; Windows NT {deviceService?.OperationSystemVersion}; Trident/{deviceService?.TridentVersion}.0)";
        }

        protected override void Reset()
        {
        }
    }
}