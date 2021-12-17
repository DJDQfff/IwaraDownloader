using System;
using System.Collections.ObjectModel;

using IwaraDownloader.Models;

using Windows.Storage.Pickers;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板
//TODO：本页未进行本地化
namespace IwaraDownloader.Pages
{
    //TODO：把这页的功能切换为从网站改名,此页暂停使用
    /// <summary> 可用于自身或导航至 Frame 内部的空白页。 </summary>
    public sealed partial class TranslateNameFromHashToTitle : Page
    {
        private ObservableCollection<ReNameWork> changes;
        private int finished = default;

        public TranslateNameFromHashToTitle ()
        {
            this.InitializeComponent();
            changes = new ObservableCollection<ReNameWork>();
        }

        private async void ChooseButton_Click (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            FileOpenPicker fileOpenPicker = new FileOpenPicker();
            fileOpenPicker.SuggestedStartLocation = PickerLocationId.Downloads;
            fileOpenPicker.ViewMode = PickerViewMode.List;
            fileOpenPicker.FileTypeFilter.Add(".mp4");
            var files = await fileOpenPicker.PickMultipleFilesAsync();
            foreach (var file in files)
            {
                ReNameWork changeName = ReNameWork.ChangeNameFactory(file);
                if (changeName != null)
                {
                    changes.Add(changeName);
                }
            }
        }

        private async void StartButton_Click (object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            foreach (var a in changes)
            {
                await a.Rename();
                finished += 1;
            }
        }
    }
}