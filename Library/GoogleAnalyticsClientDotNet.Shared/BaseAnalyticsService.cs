using GoogleAnalyticsClientDotNet.ServiceModel;
using GoogleAnalyticsClientDotNet.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GoogleAnalyticsClientDotNet
{
    public abstract class BaseAnalyticsService : IAnalyticsService
    {
        private const int MAX_BATCH_LINE = 20;
        private const int MAX_LENGTH = 1024 * 16;

        private HttpService HttpService { get; set; }

        private AutoResetEvent loadLocalTracksLock = new AutoResetEvent(true);

        protected INetworkHelper NetworkTool { get; set; }

        protected Queue<string> TempEventCollection { get; private set; }

        protected string DefaultUserAgent { get; set; }

        /// <summary>
        /// Support save cached tracks source to the custom storage.
        /// </summary>
        protected ILocalTracker LocalTracker { get; set; }

        private bool enabledHeartBeat = false;
        public bool EnabledHeartBeat
        {
            get
            {
                return enabledHeartBeat;
            }
            set
            {
                if (value != enabledHeartBeat)
                {
                    enabledHeartBeat = value;

                    if (enabledHeartBeat)
                    {
                        var task = StartHeartBeat();
                    }
                }
            }
        }

        private bool enabledSenderLoop = false;
        public bool EnabledSenderLoop
        {
            get
            {
                return enabledSenderLoop;
            }
            set
            {
                if (value != enabledSenderLoop)
                {
                    enabledSenderLoop = value;

                    if (enabledSenderLoop)
                    {
                        var task = StartSenderLoop();
                    }
                }
            }
        }

        public string TrackingID { get; set; }

        public string AppName { get; set; }

        public string AppId { get; set; }

        public string AppVersion { get; set; }

        public string AppNamespace { get; set; }

        public string UserId { get; set; }

        public string ClientId { get; set; }
        
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

        protected virtual void InitializeProcess()
        {
            HttpService = new HttpService();
            TempEventCollection = new Queue<string>();

            EnabledHeartBeat = true;
            EnabledSenderLoop = true;
        }

        protected abstract void Reset();

        protected abstract string BuildUserAgent();

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

            if (string.IsNullOrEmpty(eventItem.ClientId) && string.IsNullOrEmpty(ClientId) == false)
            {
                eventItem.ClientId = ClientId;
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
            
            if (EnabledSenderLoop == false)
            {
                // add tracks in the LocalTracker
                if (LocalTracker != null)
                {
                    LocalTracker?.WriteTracksAsync(new string[] { postContent });
                }
                else
                {
                    SendTrack(postContent);
                }
            }
            else
            {
                // add tracks in the Queue
                TempEventCollection.Enqueue(postContent);
            }
        }

        public async Task SaveTempEventsData(bool replace = false)
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

                await LocalTracker.WriteTracksAsync(TempEventCollection, replace);

                TempEventCollection.Clear();
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
            }
        }

        private async Task StartSenderLoop()
        {
            while (EnabledSenderLoop)
            {
                await Task.Delay(CommonDefine.POSITION_TIMER_INTERVAL);

                if (EnabledSenderLoop == false)
                {
                    break;
                }

                try
                {
                    loadLocalTracksLock.WaitOne();

                    await ImportEvents();

                    SendBatchTracks();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    Debug.WriteLine(ex.StackTrace);
                }
                finally
                {
                    loadLocalTracksLock.Set();
                }
            }
        }

        private async Task StartHeartBeat()
        {
            while (EnabledHeartBeat)
            {
                await Task.Delay(CommonDefine.HEART_BEAT_INTERVAL);

                if (EnabledHeartBeat == false)
                {
                    break;
                }

                TrackEvent(new EventParameter
                {
                    Category = "GAClientDotNet",
                    ScreenName = "GAClientDotNet",
                    Action = "PING_PONG",
                    Label = "SDK",
                    UserId = UserId,
                    ClientId = ClientId,
                });
            }
        }

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

                await LocalTracker.WriteTracksAsync(new string[] { "" }, true);
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
            }
        }

        private void SendTrack(string postContent, string uri = CommonDefine.GOOGLE_ANALYTICS_COLLECT_URL)
        {
            if (string.IsNullOrEmpty(postContent))
            {
                return;
            }

            if (NetworkTool.IsNetworkAvailable)
            {
                var task = HttpService.PostAsync(uri, postContent);
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
            /*
             * A maximum of 20 hits can be specified per request.
             * The total size of all hit payloads cannot be greater than 16K bytes.
             * No single hit payload can be greater than 8K bytes.
             */

            List<string> batchList = new List<string>();
            int currentLength = 0;
            int count = TempEventCollection.Count;
            int loopTimes = Convert.ToInt32(count / MAX_BATCH_LINE);

            if (loopTimes == 0)
            {
                loopTimes = 1;
            }

            int maxCount = MAX_BATCH_LINE * loopTimes;

            for (int i = 0; i < maxCount; i++)
            {
                if (TempEventCollection.Count == 0)
                {
                    break;
                }

                if (batchList.Count < MAX_BATCH_LINE && currentLength < (MAX_LENGTH - 50))
                {
                    string item = TempEventCollection.Dequeue();
                    batchList.Add(item);
                    currentLength = item.Length;
                }
                else
                {
                    // must send tracks
                    string batchTracks = string.Join("\r\n", batchList);
                    SendTrack(batchTracks, CommonDefine.GOOGLE_ANALYTICS_BATCH_URL);
                    batchList.Clear();
                    currentLength = 0;
                }
            }

            if (batchList.Count == 0)
            {
                return;
            }
            else
            {
                // send last tracks
                string batchTracks = string.Join("\r\n", batchList);
                SendTrack(batchTracks, CommonDefine.GOOGLE_ANALYTICS_BATCH_URL);
            }
        }

        public void Dispose()
        {
            Reset();

            EnabledHeartBeat = false;
            EnabledSenderLoop = false;

            TempEventCollection?.Clear();
            TempEventCollection = null;

            loadLocalTracksLock?.Reset();
            loadLocalTracksLock?.Dispose();
            loadLocalTracksLock = null;

            LocalTracker = null;

            GC.SuppressFinalize(this);
        }
    }
}