using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using IwaraDatabase.Entities;

namespace IwaraClient
{
    /// <summary> MMD相关操作 </summary>
    public static class MMDHelper
    {
        /// <summary>
        /// 解析iwara默认下载mmd名（不带扩展名），从中提取hash。
        /// 如：1571471146_RbaQ2HKzyVcoka2Y3_Source.mp4。
        /// 它由3部分组成： 1：Unix时间戳，对应文件在Iwara的时间（没有仔细验证）；
        /// 2：MMD的hash 3：清晰度，有Source、540p、360p几个档次（没有仔细验证）
        /// 每部分又由下划线连接
        /// </summary>
        /// <param name="displayname"> 文件名（不带扩展名） </param>
        /// <returns> MMD的hash </returns>
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

        // TODO 可以提到api库里面去
        /// <summary> 剔除 Title 中不能用作文件名的字符，并移除两端空白 </summary>
        /// <param name="title"> </param>
        /// <returns> </returns>
        public static string RemoveIllegalCharsAsFileName (this string title)
        {
            char[] chars = "< > / \\ | : \" * ? ".ToCharArray();
            string filetitle = title.Trim(chars).Trim();
            return filetitle;
        }

        /// <summary> 设置mmd的保存文件名 </summary>
        /// <param name="mmdfilename"> mmd名 </param>
        /// <param name="mode"> 保存方式 </param>
        /// <returns> </returns>
        public static string StorageFileName (this string mmdfilename, SaveNameMode mode)
        {
            if (mode == SaveNameMode.Title)
            {
                string newname = mmdfilename.RemoveIllegalCharsAsFileName() + ".mp4";
                return newname;
            }
            if (mode == SaveNameMode.Hash)
            {
                string newname = mmdfilename.Trim() + ".mp4";
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

        /// <summary> 获得MMD下载链接获取服务器 </summary>
        /// <param name="mmd"> MMD信息 </param>
        /// <returns>
        /// 访问这个链接，课程获得mmd的下载地址，地址有时效（未测）
        /// </returns>
        public static string ResponseDownloadsJson (this MMDInfo mmd)
        {
            return "https://" + mmd.Type + ".iwara.tv" + "/api/video/" + mmd.Hash;
        }

        /// <summary> 获取mmd下载地址 </summary>
        /// <param name="mmd"> MMD信息 </param>
        /// <param name="f"> 下载质量 ，从0到2, </param>
        /// <returns> mmd下载地址 </returns>
        public static async Task<Uri> DownloadUri (this MMDInfo mmd, int qualityIndex)
        {
            string apiuri = ResponseDownloadsJson(mmd);
            string json = await (new HttpClient()).GetStringAsync(apiuri);        // 将返回一个JSON，其包含了不同分辨率的MMD下载地址
            IwaraJson[] iwaraJsons = JsonSerializer.Deserialize<IwaraJson[]>(json); // JSON反序列化
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