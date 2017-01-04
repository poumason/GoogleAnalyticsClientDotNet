using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAnalyticsClientDotNet
{
    public partial class FileLocalTracker : ILocalTracker
    {
        private Task<IEnumerable<string>> ReadLocalFileAsync()
        {
            IEnumerable<string> content = null;
            string rawStr = string.Empty;

            try
            {
                using (FileStream stream = File.Open(SourceName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        rawStr = reader.ReadToEnd();
                    }
                }

                string[] listData = rawStr.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                if (listData != null || listData.Count() > 0)
                {
                    content = listData.ToList();
                }
            }
            catch (IOException)
            {
#if DEBUG
                throw;
#endif
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            return Task.FromResult(content);
        }

        private async Task WriteLocalFileAsync(IEnumerable<string> tracks)
        {
            if (tracks == null || tracks.Count() == 0)
            {
                return;
            }

            try
            {
                List<string> cachedData = new List<string>();
                cachedData.AddRange(tracks);
                string data = string.Join(Environment.NewLine, cachedData);

                File.WriteAllText(SourceName, data, Encoding.UTF8);
            }
            catch (IOException)
            {
#if DEBUG
                throw;
#endif
            }
            catch (UnauthorizedAccessException ex)
            {
                Debug.WriteLine(ex);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            await Task.FromResult(true);
        }
    }
}