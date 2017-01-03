using GoogleAnalyticsClientDotNet.IO;
using GoogleAnalyticsClientDotNet.Utility;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace GoogleAnalyticsClientDotNet
{
    public class AnalyticsService : BaseAnalyticsService
    {
        private StorageFile TempFile { get; set; }

        public override void Initialize(string trackingId)
        {
            NetworkTool = new NetworkHelper();
            base.Initialize(trackingId);
            DefaultUserAgent = BuildUserAgent();
        }

        protected override string BuildUserAgent()
        {
            var device = new DeviceInformationService();

            if (device.IsMobile)
            {
                return $"Mozilla/5.0 (Windows Phone 10.0; Android 6.0.1; Touch; {device.SystemManufacturer}; {device.ModelName};) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Mobile Safari/537.36 Edge/15.14393";
            }
            else
            {
                string touch = device.IsTouchEnabled ? "Touch;" : string.Empty;
                return $"Mozilla/5.0 (Windows NT 10.0; {touch}{device.SystemArchitecture}; {device.SystemManufacturer}; {device.ModelName};) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36 Edge/15.14393";
            }
        }        
        
        protected override async Task<string> ReadFile()
        {
            string content = string.Empty;
            var file = await GetGoogleAnalyticsTempFile();

            try
            {
                if (file != null)
                {
                    StorageFile target = file as StorageFile;
                    content = await FileIO.ReadTextAsync(target);
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return content;
        }

        protected override async Task WriteFile(string data)
        {
            var file = await GetGoogleAnalyticsTempFile();

            if (file == null)
            {
                return;
            }

            try
            {
                StorageFile target = file as StorageFile;
                await FileIO.WriteTextAsync(target, data);
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        private async Task<object> GetGoogleAnalyticsTempFile()
        {
            if (TempFile == null)
            {
                StorageFolder folder = await StorageHelper.GetFolder(CommonDefine.GA_FOLDER_NAME);
                TempFile = await folder.CreateFileAsync(CommonDefine.GA_TRACK_FILE_NAME, CreationCollisionOption.OpenIfExists);
            }

            return TempFile;
        }

        protected override void Reset()
        {
        }
    }
}