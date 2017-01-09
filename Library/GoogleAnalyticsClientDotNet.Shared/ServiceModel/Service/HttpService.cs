using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GoogleAnalyticsClientDotNet.ServiceModel
{
    public class HttpService : IDisposable
    {
        private HttpClient HttpInstance { get; set; }

        public HttpService()
        {
            HttpInstance = new HttpClient();
        }

        public void Dispose()
        {
            HttpInstance?.Dispose();
            HttpInstance = null;
        }

        public async Task<bool> PostAsync(string uri, string strContent)
        {
            try
            {
                if (HttpInstance == null)
                {
                    return false;
                }
                else
                {
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(uri));
                    request.Content = new StringContent(strContent, Encoding.UTF8);
                    var result = await HttpInstance.SendAsync(request);
                    return result.IsSuccessStatusCode;
                }
            }
            catch (Exception)
            {
#if DEBUG
                throw;
#endif
                return false;
            }
        }
    }
}