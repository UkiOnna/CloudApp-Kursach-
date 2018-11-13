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
    public partial class MainWindow : Window
    {
        ClientWork client;
        public MainWindow()
        {
            InitializeComponent();
            client = new ClientWork();
            client.SendCommand("start", "");
            client.GetCommand();
            Content = new LoginPage(this, client);
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            if (client.isWorking)
            {
                e.Cancel = true;
            }
            else
            {
                client.CloseConnect();
            }
            if (e.Cancel)
            {
                MessageBox.Show("Вы неможете закрыть приложение во время работы");
            }
        }

    }
}
