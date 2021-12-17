using System;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Xaml.Media.Imaging;

namespace IwaraDownloader.Models
{
    /// <summary> 视频文件信息 </summary>
    public class VideoInfo
    {
        public StorageFile StorageFile { set; get; }
        public BitmapImage BitmapImage { set; get; }

        public VideoInfo (StorageFile storageFile)
        {
            StorageFile = storageFile;
            BitmapImage = new BitmapImage();
        }

        public static async Task<VideoInfo> Factory (StorageFile storageFile)
        {
            VideoInfo info = new VideoInfo(storageFile);
            StorageItemThumbnail storageItemThumbnail = await storageFile.GetThumbnailAsync(ThumbnailMode.VideosView);
            await info.BitmapImage.SetSourceAsync(storageItemThumbnail);
            return info;
            //文件可能读取失败，这里不做处理，异常丢给调用方
        }
    }
}