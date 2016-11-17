using GoogleAnalyticsClientDotNet.ServiceModel;
using GoogleAnalyticsClientDotNet.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleAnalyticsClientDotNet
{
    public abstract class BaseAnalyticsService : IAnalyticsService, IDisposable, IAppTracking
    {
        public string TrackingID { get; set; }

        public string AppName { get; set; }

        public string AppId { get; set; }

        public string AppVersion { get; set; }

        public string AppNamespace { get; set; }

        /// <summary>
        /// Is batch send events. true: batch send; false: auto send.
        /// </summary>
        public bool IsBatchSendEvent { get; set; } = false;

        protected INetworkHelper NetworkTool { get; set; }

        protected Queue<string> TempEventCollection { get; private set; }

        protected Timer looptimer { get; private set; }

        private HttpService httpService;

        protected string DefaultUserAgent { get; set; }

        public virtual void Initialize(string trackingId)
        {
            httpService = new HttpService();
            TempEventCollection = new Queue<string>();
            TrackingID = trackingId;

            Task.Run(async () =>
            {
                await ImportEvents();
            });

            StartTimer();
        }

        public void Initialize(string trackingId, string appName, string appId, string appVersion)
        {
            AppId = appId;
            AppName = appName;
            AppVersion = appVersion;
            Initialize(trackingId);
        }

        public void TrackEvent(string categroy, string action, string label, string value, string screenName)
        {
            TrackEvent(new EventParameter
            {
                Category = categroy,
                Action = action,
                Label = label,
                Value = value,
                ScreenName = screenName
            });
        }

        public void TrackEvent(EventParameter eventItem)
        {
            if (eventItem == null)
            {
                return;
            }

            eventItem.TrakingID = TrackingID;
            eventItem.ApplicationId = AppId;
            eventItem.ApplicationName = AppName;
            eventItem.ApplicationVersion = AppVersion;

            if (string.IsNullOrEmpty(eventItem.UserAgent))
            {
                eventItem.UserAgent = DefaultUserAgent;
            }

            string postContent = string.Empty;

            var result = HttpParameterPacker.CreatePackedParameterResult(eventItem);

            if (result != null)
            {
                postContent = result.PostParameterMap.ToQueryString(true);
            }

            if (string.IsNullOrEmpty(postContent))
            {
                return;
            }

            SendTrack(postContent);
        }

        public async Task SaveTempEventsData()
        {
            try
            {
                if (TempEventCollection == null || TempEventCollection.Count == 0)
                {
                    return;
                }

                TempEventCollection collectionItem = null;
                var tempJson = await ReadFile();

                if (string.IsNullOrEmpty(tempJson) == false)
                {
                    collectionItem = JsonConvert.DeserializeObject<TempEventCollection>(tempJson);
                }

                if (collectionItem == null)
                {
                    collectionItem = new TempEventCollection();
                }

                collectionItem.Events.AddRange(TempEventCollection);
                TempEventCollection.Clear();

                string newJson = JsonConvert.SerializeObject(collectionItem);
                await WriteFile(newJson);
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
            }
        }

        #region Timer
        protected void StartTimer()
        {
            if (looptimer == null)
            {
                looptimer = new Timer(TimerInterval_Callback, null, Timeout.Infinite, CommonDefine.POSITION_TIMER_INTERVAL);
            }

            looptimer.Change(0, CommonDefine.POSITION_TIMER_INTERVAL);
        }

        protected void StopTimer()
        {
            if (looptimer != null)
            {
                looptimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void TimerInterval_Callback(object sender)
        {
            if (TempEventCollection.Count <= 0)
            {
                return;
            }

            StopTimer();
            var sendItem = TempEventCollection.Dequeue();
            SendTrack(sendItem);
            StartTimer();
        }

        #endregion

        private async Task ImportEvents()
        {
            try
            {
                TempEventCollection collectionItem = null;
                var tempJson = await ReadFile();

                if (string.IsNullOrEmpty(tempJson) == false)
                {
                    collectionItem = JsonConvert.DeserializeObject<TempEventCollection>(tempJson);
                }

                if (collectionItem == null)
                {
                    return;
                }
                else
                {
                    foreach (var item in collectionItem.Events)
                    {
                        TempEventCollection.Enqueue(item);
                    }
                }
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
            }
        }

        protected abstract Task<string> ReadFile();

        protected abstract Task WriteFile(string data);

        protected abstract void Reset();

        protected abstract string BuildUserAgent();

        private void SendTrack(string postContent)
        {
            if (string.IsNullOrEmpty(postContent))
            {
                return;
            }

            if (NetworkTool.IsNetworkAvailable && IsBatchSendEvent)
            {
                var task = httpService.PostAsync(CommonDefine.GOOGLE_ANALYTICS_COLLECT_URl, postContent);
                Debug.WriteLine("GoogleAnalytics: Send");
            }
            else
            {
                TempEventCollection.Enqueue(postContent);
                Debug.WriteLine("GoogleAnalytics: Enqueue");
            }
        }

        public void Dispose()
        {
            Reset();
            StopTimer();
            looptimer?.Dispose();
            looptimer = null;

            GC.SuppressFinalize(this);
        }
    }
}