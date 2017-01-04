using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleAnalyticsClientDotNet
{
    public interface ILocalTracker
    {
        string SourceName { get; set; }

        string Name { get; set; }

        Task WriteTracksAsync(IEnumerable<string> tracks);

        Task<IEnumerable<string>> ReadTrackAsync();
    }
}