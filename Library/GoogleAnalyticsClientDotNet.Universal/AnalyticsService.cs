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

        /// <summary>
        /// When App be suspended, not yet sent events will be auto save.
        /// </summary>
        public bool AutoSaveEvents { get; set; } = true;

        public override void Initialize(string trackingId)
        {
            NetworkTool = new NetworkHelper();
            base.Initialize(trackingId);
            RegistAppSuspending();
        }

        private void RegistAppSuspending()
        {
            Application.Current.Suspending -= Current_Suspending;
            Application.Current.Suspending += Current_Suspending;
        }

        private void UnRegistAppSuspending()
        {
            Application.Current.Suspending -= Current_Suspending;
        }

        private async void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            if (AutoSaveEvents == false)
            {
                return;
            }

            var deferral = e.SuspendingOperation.GetDeferral();

            await SaveTempEventsData();

            deferral.Complete();
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

        protected override async Task<object> GetGoogleAnalyticsTempFile()
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
            UnRegistAppSuspending();
        }
    }
}