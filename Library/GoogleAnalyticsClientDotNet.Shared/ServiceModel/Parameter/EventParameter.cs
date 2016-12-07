namespace GoogleAnalyticsClientDotNet.ServiceModel
{
    public class EventParameter : BaseMeasurementParameter
    {
        /// <summary>
        /// Event Category.
        /// <para>Required for event hit type.</para>
        /// </summary>
        [HttpProperty("ec", HttpPropertyFor.POST)]
        public string Category { get; set; }

        /// <summary>
        /// Event Action.
        /// <para>Required for event hit type.</para>
        /// </summary>
        [HttpProperty("ea", HttpPropertyFor.POST)]
        public string Action { get; set; }

        /// <summary>
        /// Event Label.
        /// </summary>
        [HttpProperty("el", HttpPropertyFor.POST)]
        public string Label { get; set; }

        /// <summary>
        /// Event Value.
        /// </summary>
        [HttpProperty("ev", HttpPropertyFor.POST)]
        public string Value { get; set; }

        public EventParameter()
        {
            HitType = "event";
        }
    }
}