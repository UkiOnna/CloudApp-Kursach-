using Microsoft.Win32;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ClientCloud
{
    /// <summary>
    /// Логика взаимодействия для FilesWindow.xaml
    /// </summary>
    public partial class FilesWindow : Window
    {
        private ClientWork client;
        private string fileForUpload;

        public FilesWindow(ClientWork client)
        {
            InitializeComponent();
            this.client = client;

            foreach (KeyValuePair<string, string> keyValue in client.fileList)
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
                    fileForUpload = saveFileDialog.FileName;
                    var builder = new StringBuilder();
                    builder.Append(fileForUpload);
                    builder.Replace(saveFileDialog.SafeFileName, string.Empty);
                    fileForUpload = builder.ToString();

                    string fileName = (string)listFiles.SelectedItem;
                    KeyValuePair<string, string> fileElement = new KeyValuePair<string, string>();
                    foreach (KeyValuePair<string, string> keyValue in client.fileList)
                    {
                        if (fileName == keyValue.Key)
                        {
                            fileElement = keyValue;
                        }
                    }
                    Task task = client.SendMessage("DownloadFile", fileForUpload + fileElement.Key, fileElement.Value);
                    task.Wait();
                    Downloading();
                }
                else
                {
                    fileForUpload = "";
                }
            }

            else
            {
                MessageBox.Show("Выберите файл который хотите скачать");
            }
        }

        private void Downloading()
        {
            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                Dispatcher.Invoke(() => loading.Visibility = Visibility.Visible);
                while (client.downloadSuccess == null)
                {
                }
                Dispatcher.Invoke(() => loading.Visibility = Visibility.Hidden);
                Dispatcher.Run();
            }));

            newWindowThread.SetApartmentState(ApartmentState.STA);

            newWindowThread.IsBackground = true;

            newWindowThread.Start();
        }

        private void UploadClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                fileForUpload = openFileDialog.FileName;


                string folderForUload;
                if (listFiles.SelectedItem != null)
                {
                    string fileName = (string)listFiles.SelectedItem;
                    KeyValuePair<string, string> fileElement = new KeyValuePair<string, string>();
                    foreach (KeyValuePair<string, string> keyValue in client.fileList)
                    {
                        if (fileName == keyValue.Key)
                        {
                            fileElement = keyValue;
                        }
                    }
                    folderForUload = fileElement.Value;
                }
                else
                {
                    folderForUload = "";
                }

                Task task = client.SendMessage("UploadFile", fileForUpload, folderForUload + "/" + openFileDialog.SafeFileName);
                task.Wait();
                Downloading();
            }
            else
            {
                fileForUpload = "/";
            }



        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            listFiles.Items.Clear();
            Task task = client.SendMessage("GetFiles", "");
            task.Wait();
            Downloading();
            foreach (KeyValuePair<string, string> keyValue in client.fileList)
            {
                listFiles.Items.Add(keyValue.Key);
            }
        }
    }
}
