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
 


        public LogKey(Window window,ClientWork client)
        {
            InitializeComponent();
            this.window = window;
            this.client = client;
            
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
                client.SendMessage("GetKey", key);
            }

        }

        public void Ex()
        {
            MessageBox.Show("Вы ввели неверный ключ");
        }

        private void NextPage(object sender, RoutedEventArgs e)
        {
            if (client.IsKey)
            {
                window.Content = new MainPage(window, client);
            }
            else
            {
                Ex();
            }
        }
    }

}
