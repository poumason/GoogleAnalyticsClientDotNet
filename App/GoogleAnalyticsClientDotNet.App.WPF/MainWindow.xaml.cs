using GoogleAnalyticsClientDotNet.App.Shared;
using System;
using System.Windows;

namespace GoogleAnalyticsClientDotNet.App.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TestClient testClient;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Unloaded += MainWindow_Unloaded;
            
            App.Service.ClientId = Guid.NewGuid().ToString();

            testClient = new TestClient(App.Service);
        }

        private void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            testClient.StopTimer();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            testClient.StartTimer();
        }
    }
}
