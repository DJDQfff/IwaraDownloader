using System;
using System.Collections.Generic;
using System.Linq;

using IwaraDatabase.Entities;

namespace IwaraClient
{
    //TODO:改用正则
    /// <summary> Iwara网页源码解析工具类 </summary>
    public static class HtmlParser
    {
        public static event Action AddedEvent;

        /// <summary> 获取当月有多少页，每页最多36页（目前是的） </summary>
        /// <param name="htmlPage">
        /// html网页源码，注意，这个网页源码，务必是第0页
        /// </param>
        /// <returns> </returns>
        public static int GetPageAmount (this string htmlPage)
        {
            int index = htmlPage.IndexOf("pager-last");
            string str = htmlPage.Substring(index);

            int index2 = str.IndexOf("page=");
            string str2 = str.Substring(index2 + 5);

            int index3 = str2.IndexOf("\"");
            string str3 = str2.Substring(0, index3);

            return Convert.ToInt32(str3);
        }

        /// <summary>
        /// 解析月份网页，从中提取MMDHash信息，每页最多36个
        /// </summary>
        /// <param name="htmlPage"> 网页源码 </param>
        /// <returns> IwaraMMDHash集合 </returns>
        public static List<MMDInfo> ParseMonthOverviewPage (this string htmlPage)
        {
            List<MMDInfo> MMDs = new List<MMDInfo>();
            string type = "www";
            string heart = "<i class=\"glyphicon glyphicon-heart\"></i>";
            string eyeopen = "<i class=\"glyphicon glyphicon-eye-open\"></i>";
            string classtitle = "<h3 class=\"title\">";
            string classusername = "class=\"username\">";

            List<string> hashes = new List<string>();
            List<int> hearts = new List<int>();
            List<int> eyeopens = new List<int>();
            List<string> titles = new List<string>();
            List<string> usernames = new List<string>();

            for (int i = htmlPage.IndexOf(heart); i > -1; i = htmlPage.IndexOf(heart, i + 1))
            {
                // for loop end when i=-1 ('a' not found)
                int index = htmlPage.IndexOf("</div>", i);
                string str = htmlPage.Substring(i + heart.Length, index - i - heart.Length).Trim();
                hearts.Add(Convert.ToInt32(str));
            }

            for (int i = htmlPage.IndexOf(eyeopen); i > -1; i = htmlPage.IndexOf(eyeopen, i + 1))
            {
                // for loop end when i=-1 ('a' not found)
                int index = htmlPage.IndexOf("</div>", i);
                string str = htmlPage.Substring(i + eyeopen.Length, index - i - eyeopen.Length).Trim();
                string open = str.Trim('k');
                eyeopens.Add((int) (double.Parse(open) * 1000));
            }

            for (int i = htmlPage.IndexOf(classtitle); i > -1; i = htmlPage.IndexOf(classtitle, i + 1))
            {
                // for loop end when i=-1 ('a' not found)
                int index = htmlPage.IndexOf("</a>", i);
                string str = htmlPage.Substring(i + classtitle.Length, index - i - classtitle.Length).Trim();

                int index2 = htmlPage.LastIndexOf("\">", index);
                int index3 = htmlPage.LastIndexOf('/', index2);
                string hash = htmlPage.Substring(index3 + 1, index2 - index3 - 1);
                string title = htmlPage.Substring(index2 + 2, index - index2 - 2);
                hashes.Add(hash);
                titles.Add(title);
            }

            for (int i = htmlPage.IndexOf(classusername); i > -1; i = htmlPage.IndexOf(classusername, i + 1))
            {
                // for loop end when i=-1 ('a' not found)
                int index = htmlPage.IndexOf("</a>", i);
                string str = htmlPage.Substring(i + classusername.Length, index - i - classusername.Length).Trim();
                usernames.Add(str);
            }

            if (htmlPage.Contains("ecchi.iwara.tv"))
                type = "ecchi";

            if (hashes.Count != titles.Count || titles.Count != hearts.Count || hearts.Count != eyeopens.Count || eyeopens.Count != usernames.Count)
                throw new Exception("网页解析出错啦");

            for (int i = 0; i < hashes.Count; i++)
            {
                var b = MMDs.Where(n => n.Hash == hashes[i]).Count();//检查网页内自己有没有重复项
                if (b == 0)
                {
                    MMDs.Add(new MMDInfo
                    {
                        EyeOpen = eyeopens[i],
                        Hash = hashes[i],
                        Title = titles[i],
                        Heart = hearts[i],
                        Username = usernames[i],
                        WhetherDownloaded = false,
                        Type = type
                    });
                    AddedEvent?.Invoke();
                }
            }

            return MMDs;
        }
    }
}