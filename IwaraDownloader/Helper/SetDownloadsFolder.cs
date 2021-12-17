using System;
using System.Threading.Tasks;

using Windows.Storage;

using static Windows.Storage.AccessCache.StorageApplicationPermissions;

namespace IwaraDownloader.Helper
{
    public static class SetDownloadsFolder
    {
        public static void SetSaveFolder (StorageFolder storageFolder)
        {
            string path = storageFolder.Path;
            ApplicationData.Current.LocalSettings.Values["SaveFolderPath"] = path;

            string token = FutureAccessList.Add(storageFolder);
            ApplicationData.Current.LocalSettings.Values["SaveFolderToken"] = token;
        }

        public static string GetSaveFolderToken ()
        {
            string token = ApplicationData.Current.LocalSettings.Values["SaveFolderToken"] as string;
            return token;
        }

        public static string GetSaveFolderPath ()
        {
            string path = ApplicationData.Current.LocalSettings.Values["SaveFolderPath"] as string;
            return path;
        }

        public static async Task<StorageFolder> GetSaveFolderAsync ()
        {
            string token = GetSaveFolderToken();
            StorageFolder storageFolder = await FutureAccessList.GetFolderAsync(token);
            return storageFolder;
        }

        public static async Task EnsureFolderExits ()
        {
            try
            {
                _ = await GetSaveFolderAsync();//如果文件夹不存在，则会报错
            }
            catch (Exception)
            {
                StorageFolder newfolder = await Windows.Storage.DownloadsFolder.CreateFolderAsync("MMD", CreationCollisionOption.GenerateUniqueName);
                SetSaveFolder(newfolder);
            }
        }
    }
}