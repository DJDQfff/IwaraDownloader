using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using IwaraDownloader.ContentDialogs;
using IwaraDownloader.Helper;

using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace IwaraDownloader.Pages
{
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class Setting : Page
    {
        private Windows.ApplicationModel.Resources.ResourceLoader resourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
        public Setting ()
        {
            this.InitializeComponent();
            CountSlider.AddHandler(PointerReleasedEvent, new Windows.UI.Xaml.Input.PointerEventHandler(CountSlider_PointerReleased), true);
        }

        protected override void OnNavigatedTo (NavigationEventArgs e)
        {
            pathText.Text = SetDownloadsFolder.GetSaveFolderPath();
            CountSlider.Value = MaxDownloads.GetCount();
        }

        private void CountSlider_PointerReleased (object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Slider slider = sender as Slider;
            double newvalud = slider.Value;

            int value = Convert.ToInt32(newvalud);

            MaxDownloads.SetCount(value);
        }

        private async void PathButton_Click (object sender, RoutedEventArgs e)
        {
            StorageFolder oldFolder = await SetDownloadsFolder.GetSaveFolderAsync();

            StorageFolder newFolder = await PickFolder.PickSaveFolderAsync();
            if (newFolder != null)
            {
                SetDownloadsFolder.SetSaveFolder(newFolder);
                pathText.Text = newFolder.Path;
                WhetherMoveDownloadedMMD whetherMoveDownloadedMMD = new WhetherMoveDownloadedMMD();
                var a = await whetherMoveDownloadedMMD.ShowAsync();
                if (a == ContentDialogResult.Primary)
                    await MoveMMD(oldFolder, newFolder);
            }
        }

        private async void OpenButton_Click (object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchFolderPathAsync(pathText.Text);
        }

        private async Task MoveMMD (StorageFolder oldfolder, StorageFolder newfolder)
        {
            var storageItems = await oldfolder.GetFilesAsync();
            List<Task> tasks = new List<Task>();
            foreach (var item in storageItems)
            {
                tasks.Add(Move(item, newfolder));
            }
            await Task.WhenAll(tasks);

            async Task Move (StorageFile item, StorageFolder folder)
            {
                await item.MoveAsync(folder, item.Name, NameCollisionOption.GenerateUniqueName);
            }
        }
    }
}