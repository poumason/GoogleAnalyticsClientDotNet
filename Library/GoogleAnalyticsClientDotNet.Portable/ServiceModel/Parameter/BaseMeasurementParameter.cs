using System.Globalization;

namespace GoogleAnalyticsClientDotNet.ServiceModel
{
    /// <summary>
    /// https://developers.google.com/analytics/devguides/collection/protocol/v1/parameters
    /// </summary>
    public class BaseMeasurementParameter
    {
        #region General

        [HttpProperty("v", HttpPropertyFor.POST)]
        public string Version { get; set; } = "1";

        [HttpProperty("tid", HttpPropertyFor.POST)]
        internal string TrakingID { get; set; }

        [HttpProperty("aip", HttpPropertyFor.POST, true)]
        public string AnonymizeIP { get; set; }

        [HttpProperty("ds", HttpPropertyFor.POST)]
        public string DataSource { get; set; } = "app";

        [HttpProperty("qt", HttpPropertyFor.POST)]
        public int QueueTime { get; set; } = 0;

        [HttpProperty("z", HttpPropertyFor.POST)]
        public string CacheBuster { get; set; }

        #endregion

        #region User

        /// <summary>
        /// <para>This anonymously identifies a particular user, device, or browser instance.</para>
        /// <para>Required for all hit types.</para>
        /// </summary>
        [HttpProperty("cid", HttpPropertyFor.POST)]
        public string ClientId { get; set; }

        /// <summary>
        /// <para>This is intended to be a known identifier for a user provided by the site owner/tracking library user. </para>
        /// <para>Optional.</para>
        /// </summary>
        [HttpProperty("uid", HttpPropertyFor.POST)]
        public string UserId { get; set; }

        #endregion

        #region Session

        [HttpProperty("sc", HttpPropertyFor.POST)]
        public string SessionControl { get; set; }

        [HttpProperty("uip", HttpPropertyFor.POST)]
        public string IpOverride { get; set; }

        [HttpProperty("ua", HttpPropertyFor.POST)]
        public string UserAgent { get; set; } = string.Empty;

        [HttpProperty("geoid", HttpPropertyFor.POST)]
        public string GeographicalOverride { get; set; }
        #endregion

        #region SystemInfo

        [HttpProperty("sr", HttpPropertyFor.POST)]
        public string ScreenResolution { get; set; }

        [HttpProperty("vp", HttpPropertyFor.POST)]
        public string ViewportSize { get; set; }

        [HttpProperty("de", HttpPropertyFor.POST)]
        public string DocumentEncoding { get; set; } = "utf-8";

        [HttpProperty("ul", HttpPropertyFor.POST)]
        public string UserLanguage
        {
            get
            {
                return CultureInfo.CurrentUICulture.Name.ToLower();
            }
        }

        #endregion

        #region Hint

        /// <summary>
        /// The type of hit.Must be one of 'pageview', 'screenview', 'event', 'transaction', 'item', 'social', 'exception', 'timing'.
        /// </summary>
        [HttpProperty("t", HttpPropertyFor.POST)]        
        public string HintType { get; set; }

        [HttpProperty("cd", HttpPropertyFor.POST)]
        public string ScreenName { get; set; }

        #endregion

        #region App Tracking

        [HttpProperty("an", HttpPropertyFor.POST)]
        public string ApplicationName { get; set; }

        [HttpProperty("aid", HttpPropertyFor.POST)]
        public string ApplicationId { get; set; }

        [HttpProperty("av", HttpPropertyFor.POST)]
        public string ApplicationVersion { get; set; }

        [HttpProperty("aiid", HttpPropertyFor.POST)]
        public string ApplicationInstallerId { get; set; }

        #endregion
    }
}