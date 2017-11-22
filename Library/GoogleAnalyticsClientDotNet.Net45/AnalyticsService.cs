using GoogleAnalyticsClientDotNet.Utility;

namespace GoogleAnalyticsClientDotNet
{
    public class AnalyticsService : BaseAnalyticsService
    {
        private DeviceInformationService deviceService;

        protected override void InitializeProcess()
        {
            NetworkTool = new NetworkHelper();

            if (LocalTracker == null)
            {
                LocalTracker = new FileLocalTracker();
            }
            
            deviceService = new DeviceInformationService();

            DefaultUserAgent = BuildUserAgent();
        }

        protected override string BuildUserAgent()
        {
            return $"Mozilla/5.0 (compatible; MSIE {deviceService?.IEVersion}.0; Windows NT {deviceService?.OperationSystemVersion}; Trident/{deviceService?.TridentVersion}.0)";
        }

        protected override void Reset()
        {
        }
    }
}