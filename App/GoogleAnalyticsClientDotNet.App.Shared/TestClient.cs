using GoogleAnalyticsClientDotNet.ServiceModel;
using System;
#if !WINDOWS_UWP
using System.Timers;
#else
using Windows.UI.Xaml;
#endif

namespace GoogleAnalyticsClientDotNet.App.Shared
{
    public class TestClient
    {

#if !WINDOWS_UWP
        Timer loopTimer;
#else
        DispatcherTimer loopTimer;
#endif

        Random randomInstance;
        AnalyticsService service;

        public TestClient(AnalyticsService service)
        {
#if !WINDOWS_UWP
            loopTimer = new Timer();
            loopTimer.Interval = 20000;
            loopTimer.Elapsed += LoopTimer_Elapsed;
#else
            loopTimer = new DispatcherTimer();
            loopTimer.Interval = TimeSpan.FromSeconds(20);
            loopTimer.Tick += LoopTimer_Tick;
#endif

            randomInstance = new Random();

            this.service = service;
        }

        public void StartTimer()
        {
            loopTimer.Start();
        }

        public void StopTimer()
        {
            loopTimer.Stop();
        }

#if !WINDOWS_UWP
        private void LoopTimer_Elapsed(object sender, ElapsedEventArgs e)
#else
        private void LoopTimer_Tick(object sender, object e)
#endif
        {
            StopTimer();
            EventParameter eventData = new EventParameter();
            eventData.Category = "Debug_catory";
            eventData.Action = "Debug_action";
            eventData.Label = "Debug_label";
            eventData.ScreenName = "Debug_screenName";
            eventData.UserId = GetUserID();
            eventData.ClientId = Guid.NewGuid().ToString();

            service.TrackEvent(eventData);

            StartTimer();
        }

        private string GetUserID()
        {
            int idx = randomInstance.Next(0, 200);
            return $"poulin_{idx}@live.com";
        }
    }
}