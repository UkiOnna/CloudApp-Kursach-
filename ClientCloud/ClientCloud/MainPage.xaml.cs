﻿using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace ClientCloud
{
    public partial class MainPage : Page
    {
        private Window window;
        private ClientWork client;
        private string fileForUpload;
        private bool isDownload;
        private char[] letters = { '\\', '/', ':', '?', '*', '"', '|' };

        public MainPage(Window window, ClientWork client)
        {
            isDownload = false;
            InitializeComponent();
            this.window = window;
            this.client = client;
            welcome.Text += client.Name + "!";
            doButtonsActive(true);
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

                    if (fileNameForDownload[2] == 'i')
                    {
                        fileNameForDownload = fileNameForDownload.Remove(0, 6);
                        Task task = client.SendCommand("DownloadFile", fileForUpload + fileNameForDownload, fileElement.Value);
                        task.Wait();
                        isDownload = true;
                        Downloading();
                    }

                    else
                    {
                        fileNameForDownload = fileNameForDownload.Remove(0, 8);
                        Task task = client.SendCommand("DownloadFolder", fileForUpload + fileNameForDownload, fileElement.Value);
                        task.Wait();
                        isDownload = true;
                        Downloading();
                    }
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

        private void ShowLogClick(object sender, RoutedEventArgs e)
        {
            if (!client.isLogWindowOpen)
            {
                Task task = client.SendCommand("GetLog", "");
                task.Wait();
                GettingLog();
            }

            else
            {
                MessageBox.Show("Окно лога уже окрыто");
            }
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

                Task task = client.SendCommand("UploadFile", fileForUpload, folderForUload + "/" + openFileDialog.SafeFileName);
                task.Wait();
                Downloading();
            }
            else
            {
                fileForUpload = "/";
            }
        }

        private void DeleteFileClick(object sender, RoutedEventArgs e)
        {
            if (listFiles.SelectedItem != null)
            {
                var dialogResult = MessageBox.Show("Вы уверены что хотите удалить это?", "Удаление", MessageBoxButton.YesNo);

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

                    Task task = client.SendCommand("DeleteItem", fileElement.Value);
                    task.Wait();
                    Downloading();
                }
            }

            else
            {
                MessageBox.Show("Выберите файл который хотите удалить");
            }
        }

        private void RefreshClick(object sender, RoutedEventArgs e)
        {
            Task task = client.SendCommand("GetFiles", "");
            task.Wait();
            RefreshingThread();
            listFiles.Items.Clear();
        }

        private void CreateFolderButtonClick(object sender, RoutedEventArgs e)
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

                        Task task = client.SendCommand("CreateFolder", fileElement.Value + "/" + fileNameWindow.FileName);
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

        private void Downloading()
        {
            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                Dispatcher.Invoke(() => loading.IsBusy = true);
                while (client.downloadSuccess == null)
                {
                    client.isWorking = true;
                    Dispatcher.Invoke(() => doButtonsActive(false));
                }
                if (isDownload == true || client.isOperationDone == false)
                {
                    isDownload = false;
                    Dispatcher.Invoke(() => loading.IsBusy = false);
                    Dispatcher.Invoke(() => doButtonsActive(true));
                    client.isWorking = false;
                }

                else
                {
                    Task task = client.SendCommand("GetFiles", "");
                    task.Wait();
                    Dispatcher.Invoke(() => RefreshingThread());
                    Dispatcher.Invoke(() => listFiles.Items.Clear());
                }

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
                    Dispatcher.Invoke(() => doButtonsActive(false));
                    client.isWorking = true;
                }

                Dispatcher.Invoke(() => loading.IsBusy = false);
                Dispatcher.Invoke(() => doButtonsActive(true));
                client.isWorking = false;
                Dispatcher.Invoke(() => Refreshing());
                Dispatcher.Run();
            }));

            newWindowThread.SetApartmentState(ApartmentState.STA);

            newWindowThread.IsBackground = true;

            newWindowThread.Start();
        }

        public void Refreshing()
        {
            foreach (KeyValuePair<string, string> keyValue in client.fileList)
            {
                if (keyValue.Key != "fileList")
                    listFiles.Items.Add(keyValue.Key);
            }
        }

        public void doButtonsActive(bool value)
        {
            downloadButton.IsEnabled = value;
            uploadButton.IsEnabled = value;
            refreshButton.IsEnabled = value;
            deleteButton.IsEnabled = value;
            createFolderButton.IsEnabled = value;
            logButton.IsEnabled = value;
        }

        private void ListFilesMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            listFiles.SelectedIndex = -1;
        }

        private void GettingLog()
        {
            Thread newWindowThread = new Thread(new ThreadStart(() =>
            {
                Dispatcher.Invoke(() => loading.IsBusy = true);

                while (client.downloadSuccess == null)
                {
                    Dispatcher.Invoke(() => doButtonsActive(false));
                    client.isWorking = true;
                }

                Dispatcher.Invoke(() => loading.IsBusy = false);
                Dispatcher.Invoke(() => doButtonsActive(true));
                client.isWorking = false;

                if (client.isLogWindowOpen)
                {
                    Dispatcher.Invoke(() => window.Content = new LogPage(window, client));
                }

                Dispatcher.Run();
            }));

            newWindowThread.SetApartmentState(ApartmentState.STA);

            newWindowThread.IsBackground = true;

            newWindowThread.Start();
        }
    }
}
