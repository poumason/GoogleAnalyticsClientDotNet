using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleAnalyticsClientDotNet
{
    public partial class FileLocalTracker : ILocalTracker
    {
        public string SourceName { get; set; }

        public string Name { get; set; }

        public FileLocalTracker()
        {
            Name = "FileLocalTracker";
            SourceName = "default_ga_track_file.mtf";
        }

        public async Task<IEnumerable<string>> ReadTrackAsync()
        {
            return await ReadLocalFileAsync();
        }

        public async Task WriteTracksAsync(IEnumerable<string> tracks)
        {
            await WriteLocalFileAsync(tracks);
        }
    }
}