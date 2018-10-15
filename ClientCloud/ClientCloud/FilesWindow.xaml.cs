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
using System.Windows.Shapes;

namespace ClientCloud
{
    /// <summary>
    /// Логика взаимодействия для FilesWindow.xaml
    /// </summary>
    public partial class FilesWindow : Window
    {
        private ClientWork client;
        public FilesWindow(ClientWork client)
        {
            InitializeComponent();
            this.client = client;
            // listFiles.Items.Add client.fileList;
          
           for(int i = 1; i < client.fileList.Count; i++)
            {
                listFiles.Items.Add(client.fileList[i]);
            }
        }


        
    }
}
