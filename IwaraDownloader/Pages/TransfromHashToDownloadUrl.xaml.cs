using System.Collections.Generic;
using System.Collections.ObjectModel;

using IwaraDownloader.Models;

using Windows.Networking.BackgroundTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace IwaraDownloader.Pages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class TransfromHashToDownloadUrl : Page
    {
        private List<DownloadOperation> activeDownloads;
        private List<string> hashqueue;
        private ObservableCollection<NotifyProgress> progressInfos;

        private VideoDownloader videoDownloader = new VideoDownloader();

        public TransfromHashToDownloadUrl ()
        {
            this.InitializeComponent();

            StartButton.Click += SetButtonEnable;
            PauseButton.Click += SetButtonEnable;
            ResumeButton.Click += SetButtonEnable;
            CancelButton.Click += SetButtonEnable;

            videoDownloader.FinishDownloadsEvent += initial;
            progressInfos = videoDownloader.progressInfos;
            activeDownloads = videoDownloader.activeDownloads;
            hashqueue = videoDownloader.hashqueue;
        }

        private async void StartButton_Click (object sender, RoutedEventArgs e)
        {
            var a = ComboBox.SelectedIndex;
            //TODO：无法连接时，发送请求，会导致闪退，下同
            await videoDownloader.StartAsync(a);
        }

        private void PauseButton_Click (object sender, RoutedEventArgs e)
        {
            videoDownloader.Pause();
        }

        private void ResumeButton_Click (object sender, RoutedEventArgs e)
        {
            videoDownloader.Resume();
        }

        private void CancelButton_Click (object sender, RoutedEventArgs e)
        {
            videoDownloader.Cancel();
        }

        /// <summary> 设置四个按键可用性 </summary>
        /// <param name="sender"> </param>
        /// <param name="e">      </param>
        private void SetButtonEnable (object sender, RoutedEventArgs e)
        {
            AppBarButton appBarButton = sender as AppBarButton;
            switch (appBarButton.Name)
            {
                case "StartButton":
                    StartButton.IsEnabled = false;
                    PauseButton.IsEnabled = true;
                    ResumeButton.IsEnabled = false;
                    CancelButton.IsEnabled = true;
                    break;

                case "PauseButton":
                    StartButton.IsEnabled = false;
                    PauseButton.IsEnabled = false;
                    ResumeButton.IsEnabled = true;
                    CancelButton.IsEnabled = true;
                    break;

                case "ResumeButton":
                    StartButton.IsEnabled = false;
                    PauseButton.IsEnabled = true;
                    ResumeButton.IsEnabled = false;
                    CancelButton.IsEnabled = true;
                    break;

                case "CancelButton":
                    StartButton.IsEnabled = true;
                    PauseButton.IsEnabled = false;
                    ResumeButton.IsEnabled = false;
                    CancelButton.IsEnabled = false;
                    break;
            }
        }

        private void initial ()
        {
            StartButton.IsEnabled = true;
            PauseButton.IsEnabled = false;
            ResumeButton.IsEnabled = false;
            CancelButton.IsEnabled = false;
        }
    }
}