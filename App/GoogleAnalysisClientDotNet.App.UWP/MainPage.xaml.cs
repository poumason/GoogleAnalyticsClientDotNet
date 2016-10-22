using GoogleAnalyticsClientDotNet;
using GoogleAnalyticsClientDotNet.ServiceModel;
using GoogleAnalyticsClientDotNet.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GoogleAnalysisClientDotNet.App.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer timer;

        AnalyticsService service;

        DeviceInformationService deviceService;

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += MainPage_Loaded;
            Unloaded += MainPage_Unloaded;

            service = new AnalyticsService();
            service.Initialize("{tracking id}");

            deviceService = new DeviceInformationService();
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
            if (timer== null)
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