using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAnalyticsClientDotNet.ServiceModel
{
    public class TempEventCollection
    {
        [JsonProperty("events")]
        public List<string> Events { get; set; }

        public TempEventCollection()
        {
            Events = new List<string>();
        }
    }
}