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
    public partial class LogPage : Page
    {
        private Window window;
        private ClientWork client;

        public LogPage(Window win,ClientWork user)
        {
            InitializeComponent();
            window = win;
            client = user;
            if (client.LogList.Count > 1)
            {
                for (int i = 1; i < client.LogList.Count; i++)
                {
                    var items = client.LogList[i].Split('-');
                    LogElement element = new LogElement();
                    element.Name = items[0];
                    element.CreationDate = items[1];
                    element.DeletedDate = items[2];
                    logList.Items.Add(element);
                }
            }
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            client.isLogWindowOpen = false;
            window.Content = new MainPage(window, client);
        }
    }
}
