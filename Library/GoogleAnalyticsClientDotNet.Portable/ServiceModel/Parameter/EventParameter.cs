namespace GoogleAnalyticsClientDotNet.ServiceModel
{
    public class EventParameter : BaseMeasurementParameter
    {
        [HttpProperty("ec", HttpPropertyFor.POST)]
        public string Category { get; set; }

        [HttpProperty("ea", HttpPropertyFor.POST)]
        public string Action { get; set; }

        [HttpProperty("el", HttpPropertyFor.POST)]
        public string Label { get; set; }

        [HttpProperty("ev", HttpPropertyFor.POST)]
        public string Value { get; set; }

        [HttpProperty("cd", HttpPropertyFor.POST)]
        public string ScreenName { get; set; }

        public EventParameter()
        {
            HintType = "event";
        }
    }
}