using Windows.Storage;

namespace IwaraDownloader.Helper
{
    public static class MaxDownloads
    {
        public static void SetCount (int i)
        {
            ApplicationData.Current.LocalSettings.Values["DownloadsCounts"] = i;
        }

        public static int GetCount ()
        {
            return (int) ApplicationData.Current.LocalSettings.Values["DownloadsCounts"];
        }

        public static bool WhetherInitialized ()
        {
            var a = ApplicationData.Current.LocalSettings.Values["DownloadsCounts"];
            if (a == null)
                return false;
            else
                return true;
        }
    }
}