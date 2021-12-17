using System;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Pickers;

namespace IwaraDownloader.Helper
{
    public static class PickFolder
    {
        public static async Task<StorageFolder> PickSaveFolderAsync ()
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.FileTypeFilter.Add(".");
            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            var folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
                return folder;
            return null;
        }
    }
}