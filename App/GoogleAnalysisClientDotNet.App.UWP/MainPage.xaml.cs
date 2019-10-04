using GoogleAnalyticsClientDotNet.App.Shared;
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
        TestClient testClient;

        public MainPage()
        {
            this.InitializeComponent();

            Loaded += MainPage_Loaded;
            Unloaded += MainPage_Unloaded;
          
            App.Service.ClientId = Guid.NewGuid().ToString();

            testClient = new TestClient(App.Service);
        }

        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            testClient.StopTimer();
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            testClient.StartTimer();
        }
    }
}