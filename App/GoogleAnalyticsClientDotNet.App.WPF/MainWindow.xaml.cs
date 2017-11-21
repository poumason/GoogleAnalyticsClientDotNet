using GoogleAnalyticsClientDotNet.ServiceModel;
using System;
using System.Windows;
using System.Windows.Threading;

namespace GoogleAnalyticsClientDotNet.App.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Unloaded += MainWindow_Unloaded;
            
            App.Service.UserId = GetUserID();
            App.Service.ClientId = Guid.NewGuid().ToString();
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            StopTimer();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
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
                timer.Interval = TimeSpan.FromSeconds(3);
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
