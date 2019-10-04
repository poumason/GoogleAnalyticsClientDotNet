using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleAnalyticsClientDotNet
{
    public interface ILocalTracker
    {
        string SourceName { get; set; }

        Task WriteTracksAsync(IEnumerable<string> tracks, bool replace = false);

        Task<IEnumerable<string>> ReadTrackAsync();
    }
}