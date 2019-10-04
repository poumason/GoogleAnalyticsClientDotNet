using GoogleAnalyticsClientDotNet.Utility;

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
            var device = new DeviceInformationService();

            if (device.IsMobile)
            {
                DefaultUserAgent = $"Mozilla/5.0 (Windows Phone 10.0; Android 6.0.1; Touch; {device.SystemManufacturer}; {device.ModelName};) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Mobile Safari/537.36 Edge/15.14393";
            }
            else
            {
                string touch = device.IsTouchEnabled ? "Touch;" : string.Empty;
                DefaultUserAgent = $"Mozilla/5.0 (Windows NT 10.0; {touch}{device.SystemArchitecture}; {device.SystemManufacturer}; {device.ModelName};) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36 Edge/15.14393";
            }
        }

        protected override void Reset()
        {
        }
    }
}