using Microsoft.Win32;
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
        private string downloadPath;
        public FilesWindow(ClientWork client)
        {
            InitializeComponent();
            this.client = client;
          
           foreach(KeyValuePair<string,string> keyValue in client.fileList)
            {
                listFiles.Items.Add(keyValue.Key);
            }
        }

        private void DownloadClick(object sender, RoutedEventArgs e)
        {
            if (listFiles.SelectedItem != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                if (saveFileDialog.ShowDialog() == true)
                {
                    downloadPath = saveFileDialog.FileName;
                    var builder = new StringBuilder();
                    builder.Append(downloadPath);
                    builder.Replace(saveFileDialog.SafeFileName, string.Empty);
                    downloadPath = builder.ToString();
                }
                else
                {
                    downloadPath = "";
                }
                string fileName = (string)listFiles.SelectedItem;
                KeyValuePair<string, string> fileElement=new KeyValuePair<string, string>();
                foreach (KeyValuePair<string, string> keyValue in client.fileList)
                {
                    if (fileName == keyValue.Key)
                    {
                        fileElement = keyValue;
                    }
                }

                Task task = client.SendMessage("DownloadFile", downloadPath+fileElement.Key,fileElement.Value);
                task.Wait();
            }
            else
            {
                MessageBox.Show("Выберите файл который хотите скачать");
            }
        }
    }
}
