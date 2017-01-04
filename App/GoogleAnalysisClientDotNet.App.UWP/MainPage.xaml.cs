using GoogleAnalyticsClientDotNet.ServiceModel;
using GoogleAnalyticsClientDotNet.Utility;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GoogleAnalysisClientDotNet.App.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer timer;

        DeviceInformationService deviceService;

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += MainPage_Loaded;
            Unloaded += MainPage_Unloaded;
          
            deviceService = new DeviceInformationService();

            App.Service.UserId = GetUserID();
            App.Service.ClientId = Guid.NewGuid().ToString();
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            StopTimer();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            StartTimer();
        }

        private void StopTimer()
        {
            timer?.Stop();
        }

        private void StartTimer()
        {
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(20);
                timer.Tick += Timer_Tick;
            }
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            StopTimer();

            EventParameter eventData = new EventParameter();
            eventData.Category = "Debug_catory";
            eventData.Action = "Debug_action";
            eventData.Label = "Debug_label";
            eventData.ScreenName = "Debug_screenName";
            eventData.UserId = GetUserID();
            eventData.ClientId = Guid.NewGuid().ToString();

            App.Service.TrackEvent(eventData);

            StartTimer();
        }

        Random randomInstance = new Random();

        private string GetUserID()
        {
            int idx = randomInstance.Next(0, 200);
            return $"poulin_{idx}@live.com";
        }
    }
}