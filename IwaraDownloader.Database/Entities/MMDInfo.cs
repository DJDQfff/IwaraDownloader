namespace IwaraDatabase.Entities
{
    /// <summary>
    /// MMD信息类
    /// </summary>
    public class MMDInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { set; get; }
        /// <summary>
        /// MMD的hash
        /// </summary>
        public string Hash { set; get; }
        /// <summary>
        /// 他属于哪个Iwara服务器，www或者ecchi
        /// </summary>
        public string Type { set; get; }
        /// <summary>
        /// Unxi时间戳
        /// </summary>
        public string UnixTimeStamp { set; get; }
        /// <summary>
        /// 作者昵称
        /// </summary>
        public string Username { set; get; }
        /// <summary>
        /// MMD名称
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 点亮红星这个MMD的人数（不固定）
        /// </summary>
        public int Heart { set; get; }
        /// <summary>
        /// 观看过这个MMD的人数
        /// </summary>
        public int EyeOpen { set; get; }
        /// <summary>
        /// 这个MMD 是否被窝下载过
        /// </summary>
        public bool WhetherDownloaded { set; get; }
    }
}