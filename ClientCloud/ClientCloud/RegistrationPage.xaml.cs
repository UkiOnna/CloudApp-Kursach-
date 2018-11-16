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
    public partial class RegistrationPage : Page
    {
        private Window window;
        private string key;
        private ClientWork client;
        private char[] letters = { '\\', '/', ':', '?', '*', '"', '|', ' ' };

        public RegistrationPage(Window window, ClientWork client)
        {
            InitializeComponent();
            this.window = window;
            this.client = client;
            keyText.Text = "6LauE3AGhBAAAAAAAAAAGTcpkyF9Dvq4NSHoJ7Cx-beAk8jZMZ7xHH8U3GExp2Va";
            registrationButton.IsEnabled = true;
        }

        private void Registration(object sender, RoutedEventArgs e)
        {
            key = keyText.Text;

            if (key == string.Empty || loginText.Text == string.Empty || passwordText.Text.Length < 4 || loginText.Text.Any(symbol => letters.Any(sub => sub == symbol)) == true
                || passwordText.Text.Any(symbol => letters.Any(sub => sub == symbol)) == true || loginText.Text.Length < 1)
            {
                Ex();
            }

            else
            {
                Task task = client.SendCommand("GetKey", key);
                task.Wait();
                CheckingKey();
            }
        }

        public void Ex()
        {
            MessageBox.Show("Вы ввели неверные данные");
        }

        private void CheckingKey()
        {
            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                Dispatcher.Invoke(() => loading.IsBusy = true);

                while (client.IsKey == null)
                {
                    client.isWorking = true;
                    Dispatcher.Invoke(() => registrationButton.IsEnabled = false);
                    Dispatcher.Invoke(() => backButton.IsEnabled = false);
                }

                Dispatcher.Invoke(() => loading.IsBusy = false);

                if (client.IsKey == true)
                {
                    Dispatcher.Invoke(() => CheckingRegistration());
                }

                else
                {
                    client.isWorking = false;
                    Dispatcher.Invoke(() => registrationButton.IsEnabled = true);
                    Dispatcher.Invoke(() => backButton.IsEnabled = true);
                }

                Dispatcher.Run();
            }));

            newWindowThread.SetApartmentState(ApartmentState.STA);

            newWindowThread.IsBackground = true;

            newWindowThread.Start();
        }

        private void CheckingRegistration()
        {
            Task task = client.SendCommand("Registration", loginText.Text, passwordText.Text);
            task.Wait();
            Registrating();
        }

        private void Registrating()
        {
            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                Dispatcher.Invoke(() => loading.IsBusy = true);

                while (client.IsRegistration == null)
                {
                    client.isWorking = true;
                    Dispatcher.Invoke(() => registrationButton.IsEnabled = false);
                    Dispatcher.Invoke(() => backButton.IsEnabled = false);
                }

                Dispatcher.Invoke(() => loading.IsBusy = false);

                if (client.IsRegistration == true)
                {
                    client.isWorking = false;
                    Dispatcher.Invoke(() => window.Content = new LoginPage(window, client));
                }

                else
                {
                    client.isWorking = false;
                    Dispatcher.Invoke(() => registrationButton.IsEnabled = true);
                    Dispatcher.Invoke(() => backButton.IsEnabled = true);
                }

                Dispatcher.Run();
            }));

            newWindowThread.SetApartmentState(ApartmentState.STA);

            newWindowThread.IsBackground = true;

            newWindowThread.Start();
        }

        private void backButtonClick(object sender, RoutedEventArgs e)
        {
            window.Content = new LoginPage(window, client);
        }
    }
}
