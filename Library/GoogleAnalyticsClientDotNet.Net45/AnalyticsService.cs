using GoogleAnalyticsClientDotNet.Utility;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAnalyticsClientDotNet
{
    public class AnalyticsService : BaseAnalyticsService
    {
        protected override void InitializeProcess()
        {
            NetworkTool = new NetworkHelper();
            DefaultUserAgent = BuildUserAgent();

            if (LocalTracker == null)
            {
                LocalTracker = new FileLocalTracker();
            }
        }

        protected override string BuildUserAgent()
        {
            var device = new DeviceInformationService();
            return $"Mozilla/5.0 (compatible; MSIE {device.IEVersion}.0; Windows NT {device.OperationSystemVersion}; Trident/{device.TridentVersion}.0)";
        }

        protected override void Reset()
        {
        }
    }
}