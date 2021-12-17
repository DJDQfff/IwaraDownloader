using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using IwaraDownloader.Helper;
using IwaraDownloader.Models;

using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace IwaraDownloader.Pages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class DownloadedVideo : Page
    {
        private StorageFolder StorageFolder;
        private ObservableCollection<VideoInfo> mp4Infos;

        public DownloadedVideo ()
        {
            this.InitializeComponent();
            mp4Infos = new ObservableCollection<VideoInfo>();
        }

        protected override void OnNavigatedTo (NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            RefreshButton_Click(null, null);
        }

        private async void RefreshButton_Click (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            mp4Infos.Clear();
            StorageFolder = await SetDownloadsFolder.GetSaveFolderAsync();
            var files = await StorageFolder.GetFilesAsync();
            List<Task> tasks = new List<Task>();
            //TODO:后期视情况而定，是否需要提前筛选一遍（是否为MP4，且文件完整，且没有被下载任务占用）
            //DONE:下载中任务不加后缀，下载中和完整MP4不需要考虑下载中的MP4情况，只需要检查是否为MP4就行
            foreach (StorageFile storageFile in files)
            {
                if (storageFile.FileType == ".mp4")//筛选MP4文件
                {
                    Task task = AddVideoAsync(storageFile);
                    tasks.Add(task);
                }
            }
            await Task.WhenAll(tasks);
            async Task AddVideoAsync (StorageFile storageFile)
            {
                //文件读取失败，就不添加
                try
                {
                    VideoInfo mp4Info = await VideoInfo.Factory(storageFile);
                    mp4Infos.Add(mp4Info);
                }
                catch (Exception ex) { }
            }
        }

        private async void GridView_ItemClick (object sender, ItemClickEventArgs e)
        {
            VideoInfo videoInfo = e.ClickedItem as VideoInfo;
            await Windows.System.Launcher.LaunchFileAsync(videoInfo.StorageFile);
        }
    }
}