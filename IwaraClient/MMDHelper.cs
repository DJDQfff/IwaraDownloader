using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using IwaraDownloader.Databases.Entities;

namespace IwaraClient
{
    public static class MMDHelper
    {
        /// <summary>
        /// 解析iwara默认下载mmd名（不带扩展名），从中提取hash
        /// </summary>
        /// <param name="displayname"> </param>
        /// <returns> </returns>
        public static string GetHashFromOfficialVideoName (this string displayname)
        {
            char c = '_';
            string[] vs = displayname.Split(c);
            if (vs == null)
                return "0";
            if (vs.Length != 3)
                return "2";

            string hash = vs[1];
            return hash;
        }

        /// <summary> 剔除 Title 中不能用作文件名的字符，并移除两端空白 </summary>
        /// <param name="title"> </param>
        /// <returns> </returns>
        public static string RemoveIllegalCharsAsFileName (this string title)
        {
            char[] chars = "< > / \\ | : \" * ? ".ToCharArray();
            string filetitle = title.Trim(chars).Trim();
            return filetitle;
        }

        public static string StorageFileName (this string hash, SaveNameMode mode)
        {
            if (mode == SaveNameMode.Title)
            {
                string newname = hash.RemoveIllegalCharsAsFileName() + ".mp4";
                return newname;
            }
            if (mode == SaveNameMode.Hash)
            {
                string newname = hash.Trim() + ".mp4";
                return newname;
            }
            return null;
        }

        /// <summary> 文件名，以Title还是一般名称 </summary>
        /// <param name="mmd"> </param>
        /// <param name="mode"> </param>
        /// <returns> </returns>
        public static string StorageFileName (this MMDInfo mmd, SaveNameMode mode)
        {
            if (mode == SaveNameMode.Title)
            {
                string newname = mmd.Title.RemoveIllegalCharsAsFileName() + ".mp4";
                return newname;
            }
            if (mode == SaveNameMode.Hash)
            {
                string raw = mmd.Hash;
                string newname = raw.Trim() + ".mp4";
                return newname;
            }
            return null;
        }

        /// <summary> 获取mmd下载链接json </summary>
        /// <param name="mmd"> </param>
        /// <returns>
        /// Json格式，mmd下载链接集合，包含soucre，720p,360p等不同分辨率的链接
        /// </returns>
        public static string ResponseDownloadsJson (this MMDInfo mmd)
        {
            return "https://" + mmd.Type + ".iwara.tv" + "/api/video/" + mmd.Hash;
        }

        /// <summary> 获取mmd下载链接 </summary>
        /// <param name="mmd"> </param>
        /// <param name="f"> 下载质量 </param>
        /// <returns> Uri类型 </returns>
        public static async Task<Uri> DownloadUri (this MMDInfo mmd, int qualityIndex)
        {
            string apiuri = ResponseDownloadsJson(mmd);
            string json = await (new HttpClient()).GetStringAsync(apiuri);
            IwaraJson[] iwaraJsons = JsonSerializer.Deserialize<IwaraJson[]>(json);
            IwaraJson iwaraJson;
            if (iwaraJsons.Length == 3)
            {
                iwaraJson = iwaraJsons[qualityIndex];
            }
            else
            {
                iwaraJson = iwaraJsons[0];
            }
            string add = "https:" + iwaraJson.uri;

            Uri uri = new Uri(add);
            return uri;
        }

        /// <summary> 用于测试下载功能 </summary>
        /// <param name="mmd"> </param>
        /// <returns> </returns>
        public static Uri TestFtpDownloadUri (this MMDInfo mmd)
        {
            string uri2 = "ftp://127.0.0.1:21/" + mmd.Hash + ".mp4";
            string uri = "ftp://192.168.43.1:5656/FTP/" + mmd.Hash + ".mp4";
            return new Uri(uri2);
        }

        /// <summary> 从测试Uri中解析mmd的 hash </summary>
        /// <param name="uri"> </param>
        /// <returns> Uri类型 </returns>
        public static string GetHashFromTestDonloadUri (this Uri uri)
        {
            string downloaduri = uri.AbsoluteUri;

            int index = downloaduri.LastIndexOf('/');
            int index2 = downloaduri.LastIndexOf('.');

            string hash = downloaduri.Substring(index + 1, index2 - index - 1);
            return hash;
        }

        /// <summary> 从测试Uri中解析mmd的 hash </summary>
        /// <param name="uri"> </param>
        /// <returns> string类型 </returns>
        public static string GetHashFromTestDonloadUri (this string uri)
        {
            int index = uri.LastIndexOf('/');
            int index2 = uri.LastIndexOf('.');

            string hash = uri.Substring(index + 1, index2 - index - 1);
            return hash;
        }

        /// <summary> 保存文件名称方式枚举 </summary>
    }

    public enum SaveNameMode
    {
        Title,
        Hash
    }

    public class IwaraJson
    {
        /// <summary>
        /// 下载质量 目前为止分为 Source、540p、360p 三种
        /// </summary>
        public string resolution { get; set; }

        /// <summary> 相对uri 前面得加上 https: </summary>
        public Uri uri { get; set; }

        /// <summary> http媒体类型 固定为 video/mp4 </summary>
        public string mime { get; set; }
    }
}