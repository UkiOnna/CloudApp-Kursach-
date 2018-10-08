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

namespace ClientCloud
{
    /// <summary>
    /// Логика взаимодействия для LogKey.xaml
    /// </summary>
    public partial class LogKey : Page
    {
        private Window window;
        private string key;
        private ClientWork chat;


        public LogKey(Window window,ClientWork chat)
        {
            InitializeComponent();
            this.window = window;
            this.chat = chat;
        }

        private void EnterKey(object sender, RoutedEventArgs e)
        {
            key = keyEnter.Text;
            if (key == string.Empty)
            {
                ex();
            }
            else
            {
                chat.SendMessage("GetKey", key);
            }       
        }

        public void ex()
        {
            MessageBox.Show("Вы ввели неверный ключ");
        }
    }
}
