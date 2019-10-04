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

        protected List<string> TempEventCollection { get; private set; }

        public string DefaultUserAgent { get; set; }

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
                        StartHeartBeat();
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
                        StartSenderLoop();
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

        public BaseAnalyticsService()
        {
            HttpService = new HttpService();
            TempEventCollection = new List<string>();
        }

        public void Initialize(string trackingId, string appName, string appId, string appVersion)
        {
            AppId = appId;
            AppName = appName;
            AppVersion = appVersion;
            TrackingID = trackingId;

            InitializeProcess();

            EnabledHeartBeat = true;
            EnabledSenderLoop = true;
        }

        public void Initialize(string trackingId, string appName, string appId, string appVersion, ILocalTracker localTracker)
        {
            LocalTracker = localTracker;
            Initialize(trackingId, appName, appId, appVersion);
        }

        protected abstract void InitializeProcess();

        protected abstract void Reset();

        protected abstract void BuildUserAgent();

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
                    var task = SendTrackAsync(new List<string> { postContent });
                }
            }
            else
            {
                // add tracks in the List
                TempEventCollection.Add(postContent);
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

        private void StartSenderLoop()
        {
            var task = Task.Run(async () =>
            {
                while (EnabledSenderLoop)
                {
                    await Task.Delay(CommonDefine.SENDER_TIMER_INTERVAL);

                    if (EnabledSenderLoop == false)
                    {
                        break;
                    }

                    try
                    {
                        loadLocalTracksLock.WaitOne();

                        List<string> needSendList = new List<string>();

                        var previousList = await GetLocalTracks();

                        // merge local tracks
                        if (previousList != null && previousList.Count() > 0)
                        {
                            needSendList.AddRange(previousList);
                        }

                        // merge memory tracks
                        if (TempEventCollection != null && TempEventCollection.Count > 0)
                        {
                            var cacheList = TempEventCollection.ToList();
                            TempEventCollection.Clear();

                            needSendList.AddRange(cacheList);
                        }

                        SendBatchTracks(needSendList);
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
            });
        }

        private void StartHeartBeat()
        {
            var task = Task.Run(async () =>
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
            });
        }

        private async Task<IEnumerable<string>> GetLocalTracks()
        {
            if (LocalTracker == null)
            {
                return null;
            }

            try
            {
                var previousTracks = await LocalTracker?.ReadTrackAsync();

                if (previousTracks == null || previousTracks.Count() == 0)
                {
                    return null;
                }

                await LocalTracker.WriteTracksAsync(new string[] { "" }, true);

                return previousTracks.ToList();
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
                return null;
            }
        }

        private async Task SendTrackAsync(List<string> postContent)
        {
            if (postContent == null || postContent.Count == 0)
            {
                return;
            }

            if (NetworkTool.IsNetworkAvailable)
            {
                try
                {
                    string batchTracks = string.Join("\r\n", postContent);
                    await HttpService.PostAsync(CommonDefine.GOOGLE_ANALYTICS_BATCH_URL, batchTracks);
                }
                catch (Exception)
                {
#if DEBUG
                    throw;
#endif
                }

                Debug.WriteLine("GoogleAnalytics: Send");
            }
            else
            {
                if (LocalTracker != null)
                {
                    try
                    {
                        await LocalTracker.WriteTracksAsync(postContent);
                    }
                    catch (Exception)
                    {
#if DEBUG
                        throw;
#endif
                    }
                }
                else
                {
                    TempEventCollection.AddRange(postContent);
                }

                Debug.WriteLine("GoogleAnalytics: Enqueue");
            }
        }

        private void SendBatchTracks(List<string> sendTrackList)
        {
            /*
             * A maximum of 20 hits can be specified per request.
             * The total size of all hit payloads cannot be greater than 16K bytes.
             * No single hit payload can be greater than 8K bytes.
             */

            if (sendTrackList == null || sendTrackList.Count == 0)
            {
                return;
            }

            int currentLength = 0;
            List<string> batchList = new List<string>();

            foreach (var item in sendTrackList)
            {
                if (batchList.Count < MAX_BATCH_LINE && currentLength < (MAX_LENGTH - 50))
                {
                    batchList.Add(item);
                    currentLength = item.Length;
                }
                else
                {
                    // must send tracks
                    var cloneBatchList = new List<string>(batchList);
                    var task = SendTrackAsync(cloneBatchList);
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
                var task = SendTrackAsync(batchList);
            }
        }

        public void Dispose()
        {
            Reset();

            EnabledHeartBeat = false;
            EnabledSenderLoop = false;

            TempEventCollection?.Clear();

            loadLocalTracksLock?.Reset();
            loadLocalTracksLock?.Dispose();
        }
    }
}