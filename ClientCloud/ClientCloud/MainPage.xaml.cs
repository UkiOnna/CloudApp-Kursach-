using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace ClientCloud
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        private Window window;
        private ClientWork client;
        public MainPage(Window window, ClientWork client)
        {
            InitializeComponent();
            this.window = window;
            this.client = client;
            welcome.Text += client.Name + "!";
            loading.Visibility = Visibility.Hidden;
        }

        private void GetFiles(object sender, RoutedEventArgs e)
        {
            Task task = client.SendMessage("GetFiles", "");
            task.Wait();
            StartingOpen();
            // window.Content = new MainPage(window, client);

        }

        public async void StartingOpen()
        {
            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                // Create and show the Window
                Dispatcher.Invoke(() => loading.Visibility = Visibility.Visible);
                while (client.fileList == null)
                {
                }
                Dispatcher.Invoke(() => loading.Visibility = Visibility.Collapsed);
               
                //Thread t = Thread.CurrentThread;
                //t.SetApartmentState(ApartmentState.STA);
                //t.Start();
                FilesWindow w = new FilesWindow(client);
                w.Show();
                // Start the Dispatcher Processing
                System.Windows.Threading.Dispatcher.Run();
            }));
            newWindowThread.SetApartmentState(ApartmentState.STA);
            // Make the thread a background thread
            newWindowThread.IsBackground = true;
            // Start the thread
            newWindowThread.Start();


        }

    }
}
