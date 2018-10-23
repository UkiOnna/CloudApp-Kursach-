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
    /// Логика взаимодействия для LogKey.xaml
    /// </summary>
    /// 

    public partial class LogKey : Page
    {
        private Window window;
        private string key;
        private ClientWork client;



        public LogKey(Window window, ClientWork client)
        {
            InitializeComponent();
            this.window = window;
            this.client = client;
            keyEnter.Text = "6LauE3AGhBAAAAAAAAAAGTcpkyF9Dvq4NSHoJ7Cx-beAk8jZMZ7xHH8U3GExp2Va";
            buttonKey.IsEnabled = true;


        }

        private void EnterKey(object sender, RoutedEventArgs e)
        {
            key = keyEnter.Text;
            if (key == string.Empty)
            {
                Ex();
            }
            else
            {
                client.IsKey = null;
                Task task = client.SendMessage("GetKey", key);
                task.Wait();
                CreatingPage();
            }

        }

        public void Ex()
        {
            MessageBox.Show("Вы ввели неверный ключ");
        }

        private void CreatingPage()
        {
            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                Dispatcher.Invoke(() => loading.Visibility = Visibility.Visible);
                while (client.IsKey == null)
                {
                    client.isWorking = true;
                    Dispatcher.Invoke(() => buttonKey.IsEnabled = false);
                }
                Dispatcher.Invoke(() => loading.Visibility = Visibility.Hidden);
                if (client.IsKey == true)
                {
                    client.isWorking = false;
                    Dispatcher.Invoke(() => window.Content = new MainPage(window, client));
                }
                Dispatcher.Run();
            }));

            newWindowThread.SetApartmentState(ApartmentState.STA);

            newWindowThread.IsBackground = true;

            newWindowThread.Start();
        }
    }

}
