//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using System.IO;
//using System.Diagnostics;
//using System.Linq;

//namespace GoogleAnalyticsClientDotNet.Utiilty
//{
//    public class FileLocalTracker : ILocalTracker
//    {
//        public string SourceName { get; set; } = "default_ga_track_file.mtf";

//        public async Task<IEnumerable<string>> ReadTrackAsync()
//        {
//            IEnumerable<string> content = null;
//            string rawStr = string.Empty;

//            try
//            {
//                using (FileStream stream = new FileStream(SourceName, FileMode.OpenOrCreate))
//                {
//                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
//                    rawStr = await reader.ReadToEndAsync();
//                }

//                string[] listData = rawStr.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

//                if (listData != null || listData.Count() > 0)
//                {
//                    content = listData.ToList();
//                }
//            }
//            catch (IOException)
//            {
//#if DEBUG
//                throw;
//#endif
//            }
//            catch (UnauthorizedAccessException ex)
//            {
//                Debug.WriteLine(ex);
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine(ex);
//            }

//            return content;
//        }


//        public async Task WriteTracksAsync(IEnumerable<string> tracks, bool replace = false)
//        {
//            if (tracks == null || tracks.Count() == 0)
//            {
//                return;
//            }

//            try
//            {
//                List<string> cachedData = new List<string>();
//                cachedData.AddRange(tracks);

//                if (replace == false)
//                {
//                    var previousData = await ReadTrackAsync();
//                    if (previousData != null)
//                    {
//                        cachedData.AddRange(previousData);
//                    }
//                }

//                string data = string.Join(Environment.NewLine, cachedData);

//                using (FileStream stream = new FileStream(SourceName, FileMode.OpenOrCreate))
//                {
//                    StreamWriter writer = new StreamWriter(stream, Encoding.UTF8);
//                    await writer.WriteAsync(data);
//                }
//            }
//            catch (IOException)
//            {
//#if DEBUG
//                throw;
//#endif
//            }
//            catch (UnauthorizedAccessException ex)
//            {
//                Debug.WriteLine(ex);
//            }
//            catch (Exception ex)
//            {
//                Debug.WriteLine(ex);
//            }
//        }
//    }
//}