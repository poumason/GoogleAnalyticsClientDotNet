﻿using GoogleAnalyticsClientDotNet.ServiceModel;
using GoogleAnalyticsClientDotNet.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleAnalyticsClientDotNet
{
    public abstract class BaseAnalyticsService : IAnalyticsService, IDisposable
    {
        private const int MAX_BATCH_LINE = 5;
        private const int MAX_LENGTH = 1024 * 16;

        private HttpService httpService;

        protected INetworkHelper NetworkTool { get; set; }

        protected Queue<string> TempEventCollection { get; private set; }

        protected Timer SenderTimer { get; private set; }

        protected Timer HeartRateTimer { get; private set; }

        protected string DefaultUserAgent { get; set; }

        protected ILocalTracker LocalTracker { get; set; }

        public string TrackingID { get; set; }

        public string AppName { get; set; }

        public string AppId { get; set; }

        public string AppVersion { get; set; }

        public string AppNamespace { get; set; }

        public string UserId { get; set; }

        public string ClientId { get; set; }

        /// <summary>
        /// Is batch send events. true: batch send; false: auto send.
        /// </summary>
        public bool IsBatchSendEvent { get; set; } = false;

        public void Initialize(string trackingId, string appName, string appId, string appVersion)
        {
            AppId = appId;
            AppName = appName;
            AppVersion = appVersion;
            TrackingID = trackingId;
            InitializeProcess();
        }

        public void Initialize(string trackingId, string appName, string appId, string appVersion, ILocalTracker localTracker)
        {
            LocalTracker = localTracker;
            Initialize(trackingId, appName, appId, appVersion);
        }

        public void TrackScreen(string screenName)
        {
            TrackEvent(new ScreenParameter
            {
                ScreenName = screenName,
                UserId = UserId,
                ClientId = ClientId
            });
        }

        public void TrackEvent(string categroy, string action, string label, string value, string screenName)
        {
            TrackEvent(new EventParameter
            {
                Category = categroy,
                Action = action,
                Label = label,
                Value = value,
                ScreenName = screenName,
                UserId = UserId,
                ClientId = ClientId
            });
        }

        public void TrackEvent(BaseMeasurementParameter eventItem)
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

            if (string.IsNullOrEmpty(eventItem.UserId) && string.IsNullOrEmpty(UserId) == false)
            {
                eventItem.UserId = UserId;
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

            if (IsBatchSendEvent)
            {
                TempEventCollection.Enqueue(postContent);
            }
            else
            {
                SendTrack(postContent);
            }
        }

        public async Task SaveTempEventsData()
        {
            try
            {
                if (TempEventCollection == null || TempEventCollection.Count == 0)
                {
                    return;
                }

                if (LocalTracker == null)
                {
                    return;
                }

                await LocalTracker.WriteTracksAsync(TempEventCollection);

                TempEventCollection.Clear();
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
            }
        }

        #region Timer
        protected void StartSenderTimer()
        {
            if (SenderTimer == null)
            {
                SenderTimer = new Timer(SenderTimerInterval_Callback, null, Timeout.Infinite, CommonDefine.POSITION_TIMER_INTERVAL);
            }

            SenderTimer.Change(0, CommonDefine.POSITION_TIMER_INTERVAL);
        }

        protected void StopSenderTimer()
        {
            if (SenderTimer != null)
            {
                SenderTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void SenderTimerInterval_Callback(object sender)
        {
            if (TempEventCollection.Count <= 0)
            {
                return;
            }

            StopSenderTimer();

            if (TempEventCollection.Count > MAX_BATCH_LINE)
            {
                SendBatchTracks();
            }
            else
            {
                var sendItem = TempEventCollection.Dequeue();
                SendTrack(sendItem);
            }

            StartSenderTimer();
        }

        private void StartHeartRateTimer()
        {
            if (HeartRateTimer == null)
            {
                HeartRateTimer = new Timer(HeartRateInterval_Callback, null, Timeout.Infinite, CommonDefine.HEART_RATE_INTERVAL);
            }

            HeartRateTimer.Change(0, CommonDefine.HEART_RATE_INTERVAL);
        }

        private void StopHeartRateTimer()
        {
            if (HeartRateTimer != null)
            {
                HeartRateTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        private void HeartRateInterval_Callback(object sender)
        {
            TrackEvent(new EventParameter
            {
                Category = "GAClientDotNet",
                ScreenName = "GAClientDotNet",
                Action = "PING_PUNG",
                Label = "SDK",
                UserId = UserId,
                ClientId = ClientId,
            });
        }

        #endregion

        protected virtual void InitializeProcess()
        {
            httpService = new HttpService();
            TempEventCollection = new Queue<string>();

            Task.Run(async () =>
            {
                await ImportEvents();
            });

            StartSenderTimer();
            StartHeartRateTimer();
        }

        protected abstract void Reset();

        protected abstract string BuildUserAgent();

        private async Task ImportEvents()
        {
            try
            {
                var previousTracks = await LocalTracker.ReadTrackAsync();

                if (previousTracks == null || previousTracks.Count() == 0)
                {
                    return;
                }

                foreach (var trackItem in previousTracks)
                {
                    TempEventCollection.Enqueue(trackItem);
                }
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
            }
        }

        private void SendTrack(string postContent)
        {
            if (string.IsNullOrEmpty(postContent))
            {
                return;
            }

            if (NetworkTool.IsNetworkAvailable)
            {
                var task = httpService.PostAsync(CommonDefine.GOOGLE_ANALYTICS_COLLECT_URL, postContent);
                Debug.WriteLine("GoogleAnalytics: Send");
            }
            else
            {
                TempEventCollection.Enqueue(postContent);
                Debug.WriteLine("GoogleAnalytics: Enqueue");
            }
        }

        private void SendBatchTracks()
        {
            List<string> batchList = new List<string>();
            int currentLength = 0;

            for (int i = 0; i < MAX_BATCH_LINE; i++)
            {
                if (currentLength < (MAX_LENGTH - 50))
                {
                    string item = TempEventCollection.Dequeue();
                    batchList.Add(item);
                    currentLength = item.Length;
                }
                else
                {
                    break;
                }
            }

            if (batchList.Count == 0)
            {
                return;
            }

            string batchTracks = string.Join("\r\n", batchList);

            if (NetworkTool.IsNetworkAvailable)
            {
                var task = httpService.PostAsync(CommonDefine.GOOGLE_ANALYTICS_BATCH_URL, batchTracks);
                Debug.WriteLine("GoogleAnalytics: batch Send");
            }
            else
            {
                foreach (var item in batchList)
                {
                    TempEventCollection.Enqueue(item);
                }
                Debug.WriteLine("GoogleAnalytics: batch Enqueue");
            }
        }

        public void Dispose()
        {
            Reset();
            StopSenderTimer();
            SenderTimer?.Dispose();
            SenderTimer = null;

            StopHeartRateTimer();
            HeartRateTimer?.Dispose();
            HeartRateTimer = null;

            TempEventCollection?.Clear();
            TempEventCollection = null;

            GC.SuppressFinalize(this);
        }
    }
}