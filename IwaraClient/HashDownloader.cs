using System;
using System.Threading.Tasks;

using IwaraDatabase.Entities;

using System.Net.Http;

namespace IwaraClient
{
    /// <summary> 用于下载hash </summary>
    public class HashDownloader
    {
        public HttpClient httpClient = new HttpClient();
        private string Host;
        public DateTimeOffset DateTimeOffset { set; get; }
        public MonthInfo ThisMonth { set; get; }
        public int HashCount { set; get; }
        private int PageAmount;

        public HashDownloader (DateTimeOffset dateTime, IwaraType iwaratype)
        {
            DateTimeOffset = dateTime;
            ThisMonth = new MonthInfo() { Year = dateTime.Year, Month = dateTime.Month };

            if (iwaratype == IwaraType.www)
                Host = $"https://www.iwara.tv";
            if (iwaratype == IwaraType.ecchi)
                Host = $"https://ecchi.iwara.tv";
        }

        /// <summary> 提取所有IwaraMMDHash集合 </summary>
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

        private Uri GetUri (int amount)
        {
            string str = Host + $"/videos?f%5B0%5D=created%3A{DateTimeOffset.Year}&f%5B1%5D=created%3A{DateTimeOffset.Year}-{DateTimeOffset.Month}&page={amount}";
            Uri uri = new Uri(str);
            return uri;
        }
    }
}