using System.Globalization;

namespace GoogleAnalyticsClientDotNet.ServiceModel
{
    /// <summary>
    /// https://developers.google.com/analytics/devguides/collection/protocol/v1/parameters
    /// </summary>
    public class BaseMeasurementParameter
    {
        [HttpProperty("v", HttpPropertyFor.POST)]
        public string Version { get; set; } = "1";

        [HttpProperty("tid", HttpPropertyFor.POST)]
        internal string TrakingID { get; set; }

        //[HttpProperty("aip", HttpPropertyFor.POST, true)]
        //public string AnonymizeIP { get; set; }

        [HttpProperty("ds", HttpPropertyFor.POST)]
        public string DataSource { get; set; } = "app";

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

        [HttpProperty("ua", HttpPropertyFor.POST)]
        public string UserAgent { get; set; }

        [HttpProperty("t", HttpPropertyFor.POST)]
        public string HintType { get; set; }

        [HttpProperty("an", HttpPropertyFor.POST)]
        public string ApplicationName { get; set; }

        [HttpProperty("aid", HttpPropertyFor.POST)]
        public string ApplicationId { get; set; }

        [HttpProperty("av", HttpPropertyFor.POST)]
        public string ApplicationVersion { get; set; }

        [HttpProperty("ul", HttpPropertyFor.POST)]
        public string UserLanguage
        {
            get
            {
                return CultureInfo.CurrentUICulture.Name.ToLower();
            }
        }

        [HttpProperty("os", HttpPropertyFor.POST)]
        public string OperatingSystem { get; set; }
    }
}