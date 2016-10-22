using GoogleAnalyticsClientDotNet.ServiceModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoogleAnalyticsClientDotNet
{
    public interface IAnalyticsService
    {
        void Initialize(string trackingId);

        string TrackingID { get; set; }

        void TrackEvent(EventParameter eventItem);
        
        Task SaveTempEventsData();
    }
}