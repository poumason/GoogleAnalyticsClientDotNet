using System.Collections.Generic;

namespace GoogleAnalyticsClientDotNet.ServiceModel
{
    public class HttpParameterPackResult
    {
        public string GetCombindedString { get; set; }

        public Dictionary<string, string> HeaderParameterMap { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> PostParameterMap { get; set; } = new Dictionary<string, string>();

        public List<string> PostRawStringList { get; set; } = new List<string>();

        public byte[] PostRawData { get; set; } = null;
    }
}