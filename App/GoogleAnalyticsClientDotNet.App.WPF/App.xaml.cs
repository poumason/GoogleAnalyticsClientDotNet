using System.Windows;

namespace GoogleAnalyticsClientDotNet.App.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static AnalyticsService Service { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Initialize AnalyticsService
            Service = new AnalyticsService();
            Service.Initialize("{tracking id}", "{appName}", "{appId}", "{appVersion}");
        }

        protected async override void OnExit(ExitEventArgs e)
        {
            await Service?.SaveTempEventsData(true);

            base.OnExit(e);
        }
    }
}