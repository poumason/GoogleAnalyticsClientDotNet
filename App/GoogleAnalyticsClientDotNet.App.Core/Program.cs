using GoogleAnalyticsClientDotNet.App.Shared;
using System;

namespace GoogleAnalyticsClientDotNet.App.Core
{
    class Program
    {
        public static AnalyticsService Service { get; set; }
        private static TestClient testClient;

        static void Main(string[] args)
        {
            // Initialize AnalyticsService
            Service = new AnalyticsService();

            // Must setting default user agent
            Service.DefaultUserAgent = $"Mozilla/5.0 (Windows NT 10.0; false x64; Microsoft; Surface Book;) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.79 Safari/537.36 Edge/15.14393";

            Service.Initialize("{tracking id}", "{appName}", "{appId}", "{appVersion}");
            
            testClient = new TestClient(Service);

            testClient.StartTimer();

            Console.Write("Start send tracks ...");
            Console.ReadLine();
        }
    }
}