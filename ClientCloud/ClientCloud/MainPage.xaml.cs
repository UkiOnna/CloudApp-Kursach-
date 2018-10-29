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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ClientCloud
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    /// 
    public partial class MainPage : Page
    {
        private Window window;
        private ClientWork client;
        private string fileForUpload;
        private bool needToRefresh;
        public MainPage(Window window, ClientWork client)
        {
            InitializeComponent();
            this.window = window;
            this.client = client;
            welcome.Text += client.Name + "!";
            doButtons(true);
            Refreshing();
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
                    string fileNameForDownload = fileElement.Key.Trim();
                    fileNameForDownload = fileNameForDownload.Remove(0, 6);
                    Task task = client.SendMessage("DownloadFile", fileForUpload + fileNameForDownload, fileElement.Value);
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
                Dispatcher.Invoke(() => loading.IsBusy = true);
                while (client.downloadSuccess == null)
                {
                    client.isWorking = true;
                    Dispatcher.Invoke(() => doButtons(false));
                }
                Dispatcher.Invoke(() => loading.IsBusy=false);
                Dispatcher.Invoke(() => doButtons(true));
                client.isWorking = false;
                Dispatcher.Run();
            }));

            newWindowThread.SetApartmentState(ApartmentState.STA);

            newWindowThread.IsBackground = true;

            newWindowThread.Start();
        }

        private void RefreshingThread()
        {
            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                Dispatcher.Invoke(() => loading.IsBusy = true);
                while (client.refreshSuccess == null)
                {
                    Dispatcher.Invoke(() => doButtons(false));
                    client.isWorking = true;
                }
                Dispatcher.Invoke(() => loading.IsBusy = false);
                Dispatcher.Invoke(() => doButtons(true));
                client.isWorking = false;
                Dispatcher.Invoke(() => Refreshing());

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
            Task task = client.SendMessage("GetFiles", "");
            task.Wait();
            RefreshingThread();
            listFiles.Items.Clear();
        }

        public void Refreshing()
        {
            foreach (KeyValuePair<string, string> keyValue in client.fileList)
            {
                if (keyValue.Key != "fileList")
                    listFiles.Items.Add(keyValue.Key);
            }
        }

        public void doButtons(bool value)
        {
            downloadButton.IsEnabled = value;
            uploadButton.IsEnabled = value;
            refreshButton.IsEnabled = value;
            deleteButton.IsEnabled = value;
            createFolderButton.IsEnabled = value;
        }

        private void listFilesMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            listFiles.SelectedIndex = -1;
        }

        private void DeleteFile(object sender, RoutedEventArgs e)
        {
            if (listFiles.SelectedItem != null)
            {
                var dialogResult = MessageBox.Show("Вы уверены что хотите удалить это?","Удаление", MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
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
                    Task task = client.SendMessage("DeleteItem", fileElement.Value);
                    task.Wait();
                    Downloading();
                }
            }
            else
            {
                MessageBox.Show("Выберите файл который хотите удалить");
            }
        }

        private char[] letters = {'\\', '/', ':', '?',  '*',  '"', '|' };

        private void createFolderButtonClick(object sender, RoutedEventArgs e)
        {
            FileNameWindow fileNameWindow = new FileNameWindow();

            if (fileNameWindow.ShowDialog() == true)
            {
                if (fileNameWindow.FileName != string.Empty)
                {
                    if (!fileNameWindow.FileName.Any(symbol => letters.Any(sub => sub == symbol)))
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
                        Task task = client.SendMessage("CreateFolder", fileElement.Value + "/" + fileNameWindow.FileName);
                        task.Wait();
                        Downloading();
                    }
                    else
                    {
                        MessageBox.Show("Недопустимое имя папки");
                    }
                    
                }
            }

        }
    }
}
