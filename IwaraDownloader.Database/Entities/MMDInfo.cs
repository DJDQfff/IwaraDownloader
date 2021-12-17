namespace IwaraDownloader.Databases.Entities
{
    public class MMDInfo
    {
        public int Id { set; get; }
        public string Hash { set; get; }
        public string Type { set; get; }
        public string UnixTimeStamp { set; get; }
        public string Username { set; get; }
        public string Title { set; get; }
        public int Heart { set; get; }
        public int EyeOpen { set; get; }
        public bool WhetherDownloaded { set; get; }
    }
}