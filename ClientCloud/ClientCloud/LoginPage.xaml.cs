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
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private Window window;
        private ClientWork client;
        public LoginPage(Window window, ClientWork client)
        {
            this.window = window;
            this.client = client;
            InitializeComponent();
        }

        private void RegistrationClick(object sender, RoutedEventArgs e)
        {
            window.Content = new RegistrationPage(window, client);
        }

        private void EnterClick(object sender, RoutedEventArgs e)
        {
            Task task = client.SendMessage("Login", loginText.Text, passwordText.Password);
            task.Wait();
            Entering();
        }

        private void Entering()
        {
            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                Dispatcher.Invoke(() => loading.IsBusy = true);
                while (client.IsLogin == null)
                {
                    client.isWorking = true;
                    Dispatcher.Invoke(() => buttonEnter.IsEnabled = false);
                    Dispatcher.Invoke(() => buttonRegistration.IsEnabled = false);
                }
                Dispatcher.Invoke(() => loading.IsBusy = false);
                if (client.IsLogin == true)
                {
                    client.isWorking = false;
                    Dispatcher.Invoke(() => window.Content = new MainPage(window, client));
                }
                else
                {
                    client.isWorking = false;
                    Dispatcher.Invoke(() => buttonRegistration.IsEnabled = true);
                    Dispatcher.Invoke(() => buttonEnter.IsEnabled = true);
                }
                Dispatcher.Run();
            }));

            newWindowThread.SetApartmentState(ApartmentState.STA);

            newWindowThread.IsBackground = true;

            newWindowThread.Start();
        }
    }
}
