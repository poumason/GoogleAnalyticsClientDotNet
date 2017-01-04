using GoogleAnalyticsClientDotNet.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace GoogleAnalyticsClientDotNet
{
    public partial class FileLocalTracker : ILocalTracker
    {
        private StorageFile TempFile { get; set; }

        private async Task<IEnumerable<string>> ReadLocalFileAsync()
        {
            IEnumerable<string> content = null;

            try
            {
                var file = await GetGoogleAnalyticsTempFile();

                if (file != null)
                {
                    StorageFile target = file as StorageFile;
                    var rawStr = await FileIO.ReadTextAsync(target);

                    string[] listData = rawStr.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                    if (listData != null || listData.Count() > 0)
                    {
                        content = listData.ToList();
                    }
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

        private async Task WriteLocalFileAsync(IEnumerable<string> tracks, bool replace)
        {
            if (tracks == null || tracks.Count() == 0)
            {
                return;
            }

            try
            {
                var file = await GetGoogleAnalyticsTempFile();

                if (file == null)
                {
                    return;
                }

                List<string> cachedData = new List<string>();
                cachedData.AddRange(tracks);

                if (replace == false)
                {
                    var previousData = await ReadTrackAsync();
                    if (previousData != null)
                    {
                        cachedData.AddRange(previousData);
                    }
                }

                string data = string.Join(Environment.NewLine, cachedData);
                StorageFile target = file as StorageFile;
                await FileIO.WriteTextAsync(target, data);

                target = null;
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
                TempFile = await folder.CreateFileAsync(SourceName, CreationCollisionOption.OpenIfExists);
            }

            return TempFile;
        }
    }
}