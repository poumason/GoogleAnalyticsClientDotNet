using GoogleAnalyticsClientDotNet.Utility;
using System;

namespace GoogleAnalyticsClientDotNet
{
    public class AnalyticsService : BaseAnalyticsService
    {
        protected override void BuildUserAgent()
        {
            if (string.IsNullOrEmpty(DefaultUserAgent))
            {
                throw new ArgumentNullException("Must setting DefaultUserAgent property.");
            }
        }

        protected override void InitializeProcess()
        {
            NetworkTool = new NetworkHelper();

            if (LocalTracker == null)
            {
                LocalTracker = new FileLocalTracker();
            }
        }

        protected override void Reset()
        {
            this.NetworkTool = null;
            this.LocalTracker = null;
        }
    }
}
