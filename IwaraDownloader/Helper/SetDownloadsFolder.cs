using System;
using System.Threading.Tasks;

using Windows.Storage;

using static Windows.Storage.AccessCache.StorageApplicationPermissions;

namespace IwaraDownloader.Helper
{
    /// <summary>
    /// 系统设置：下载文件夹相关设置
    /// </summary>
    public static class SetDownloadsFolder
    {
        /// <summary>
        /// 设置下载文件夹
        /// </summary>
        /// <param name="storageFolder"></param>
        public static void SetSaveFolder (StorageFolder storageFolder)
        {
            string path = storageFolder.Path;
            ApplicationData.Current.LocalSettings.Values["SaveFolderPath"] = path;

            string token = FutureAccessList.Add(storageFolder);
            ApplicationData.Current.LocalSettings.Values["SaveFolderToken"] = token;
        }
        /// <summary>
        /// 获取下载文件夹令牌
        /// </summary>
        /// <returns></returns>
        public static string GetSaveFolderToken ()
        {
            string token = ApplicationData.Current.LocalSettings.Values["SaveFolderToken"] as string;
            return token;
        }
        /// <summary>
        /// 获取下载文件夹路径
        /// </summary>
        /// <returns></returns>
        public static string GetSaveFolderPath ()
        {
            string path = ApplicationData.Current.LocalSettings.Values["SaveFolderPath"] as string;
            return path;
        }
        /// <summary>
        /// 获取下载文件夹
        /// </summary>
        /// <returns></returns>
        public static async Task<StorageFolder> GetSaveFolderAsync ()
        {
            string token = GetSaveFolderToken();
            StorageFolder storageFolder = await FutureAccessList.GetFolderAsync(token);
            return storageFolder;
        }
        /// <summary>
        /// 保证文件夹存在
        /// </summary>
        /// <returns></returns>
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