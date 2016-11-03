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
            RegistWindowClose();            
        }

        protected override Task<object> GetGoogleAnalyticsTempFile()
        {
            return null;
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
                content = string.Empty;
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