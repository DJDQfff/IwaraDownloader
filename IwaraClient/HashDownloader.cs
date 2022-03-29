using System;
using System.Threading.Tasks;

using IwaraDatabase.Entities;

using System.Net.Http;

namespace IwaraClient
{
    /// <summary>
    /// Iwara MMD下载器类，封装IwaraMMD下载链接获取任务
    /// </summary>
    public class HashDownloader
    {
        /// <summary> 网络客户端 </summary>
        private HttpClient httpClient = new HttpClient();

        /// <summary>
        /// 要访问的网站服务器，Iwara有两个MMD访问服务器，一个WWW，一个ecchi
        /// </summary>
        private string Host;

        /// <summary> 当前要获取的月份信息 </summary>
        public DateTimeOffset DateTimeOffset { set; get; }

        /// <summary> 存存储到数据库的信息 </summary>
        public MonthInfo ThisMonth { set; get; }

        /// <summary> mmd个数统计 </summary>
        public int HashCount { set; get; }

        /// <summary> 网页页面个数统计 </summary>
        private int PageAmount;

        /// <summary> 构造函数，初始化这个月的相关信息 </summary>
        /// <param name="dateTime"> 月份信息 </param>
        /// <param name="iwaratype"> Iwara服务器类型 </param>
        public HashDownloader (DateTimeOffset dateTime, IwaraWebSiteType iwaratype)
        {
            DateTimeOffset = dateTime;
            ThisMonth = new MonthInfo() { Year = dateTime.Year, Month = dateTime.Month };

            if (iwaratype == IwaraWebSiteType.www)
                Host = $"https://www.iwara.tv";
            if (iwaratype == IwaraWebSiteType.ecchi)
                Host = $"https://ecchi.iwara.tv";
        }

        /// <summary> 操作： 提取所有IwaraMMDHash集合 </summary>
        /// <returns> </returns>
        public async Task GetAllHashes ()
        {
            Uri uri = GetUri(0);
            string HtmlPage = await httpClient.GetStringAsync(uri);
            var mmdlist = HtmlPage.ParseMonthOverviewPage();
            ThisMonth.MMDs.AddRange(mmdlist);

            PageAmount = HtmlPage.GetPageAmount();
            //TODO:改为task任务获取所有网页后，在进行解析

            for (int a = 1; a < PageAmount; a++)
            {
                HtmlPage = await httpClient.GetStringAsync(GetUri(a));
                var list1 = HtmlPage.ParseMonthOverviewPage();
                ThisMonth.MMDs.AddRange(list1);
            }
        }

        /// <summary> 获取源网页地址链接 </summary>
        /// <param name="amount"> 第几页 </param>
        /// <returns> </returns>
        private Uri GetUri (int amount)
        {
            string str = Host + $"/videos?f%5B0%5D=created%3A{DateTimeOffset.Year}&f%5B1%5D=created%3A{DateTimeOffset.Year}-{DateTimeOffset.Month}&page={amount}";
            Uri uri = new Uri(str);
            return uri;
        }
    }
}