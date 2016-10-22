using GoogleAnalyticsClientDotNet.ServiceModel;
using GoogleAnalyticsClientDotNet.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace GoogleAnalyticsClientDotNet.App.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;

        AnalyticsService service;

        DeviceInformationService deviceService;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;

            Unloaded += MainWindow_Unloaded;

            service = new AnalyticsService();
            service.Initialize("{tracking id}");

            deviceService = new DeviceInformationService();
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
                timer.Interval = TimeSpan.FromSeconds(10);
                timer.Tick += Timer_Tick;
            }
            timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            StopTimer();
            EventParameter eventData = new EventParameter();
            eventData.Category = "";
            eventData.Action = "";
            eventData.Label = "";
            eventData.ScreenName = "";
            eventData.ClientId = "";
            eventData.UserAgent = deviceService.ModelName;

            service.TrackEvent(eventData);
            StartTimer();
        }
    }
}
