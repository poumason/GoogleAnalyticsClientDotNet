using GoogleAnalyticsClientDotNet.Utility;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAnalyticsClientDotNet
{
    public class AnalyticsService : BaseAnalyticsService
    {
        public override void Initialize(string trackingId)
        {
            NetworkTool = new NetworkHelper();
            base.Initialize(trackingId);
            DefaultUserAgent = BuildUserAgent();
        }

        protected override string BuildUserAgent()
        {
            var device = new DeviceInformationService();
            return $"Mozilla/5.0 (compatible; MSIE {device.IEVersion}.0; Windows NT {device.OperationSystemVersion}; Trident/{device.TridentVersion}.0)";
        }

        protected override Task<string> ReadFile()
        {
            string content = string.Empty;

            try
            {
                content = File.ReadAllText(CommonDefine.GA_TRACK_FILE_NAME, Encoding.UTF8);
            }
            catch (IOException)
            {
#if DEBUG
                throw;
#endif
            }

            return Task.FromResult(content);
        }

        protected override Task WriteFile(string data)
        {
            try
            {
                File.WriteAllText(CommonDefine.GA_TRACK_FILE_NAME, data, Encoding.UTF8);
            }
            catch (IOException)
            {
#if DEBUG
                throw;
#endif
            }

            return Task.FromResult(true);
        }

        protected override void Reset()
        {
            UnRegistWindowClose();
        }

        private void RegistWindowClose()
        {
        }

        private void UnRegistWindowClose()
        {
        }
    }
}