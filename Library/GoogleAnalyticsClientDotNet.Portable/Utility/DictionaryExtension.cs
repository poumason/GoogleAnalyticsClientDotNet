using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAnalyticsClientDotNet.Utility
{
    public static class DictionaryExtension
    {
        public static string ToQueryString(this Dictionary<string,string> keyvalues , bool urlEncoded)
        {
            if (keyvalues == null || keyvalues.Count == 0)
            {
                return string.Empty;
            }

            List<string> builder = new List<string>();

            foreach (KeyValuePair<string, string> keyValuePair in keyvalues)
            {
                if (keyValuePair.Key == null)
                {
                    continue;
                }

                string value = keyValuePair.Value == null ? string.Empty : keyValuePair.Value;
                value = urlEncoded ? WebUtility.HtmlEncode(value) : value;

                builder.Add($"{keyValuePair.Key}={value}");
            }

            return string.Join("&", builder);
        } 
    }
}