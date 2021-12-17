using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
namespace IwaraDownloader.Models
{
    public class MonthInfo
    {
        public int Year { set; get; }
        public int Month { set; get; }
        public List<MMDInfo> IwaraMMDHashes { set; get; }
        public MonthInfo(DateTimeOffset date)
        {
            IwaraMMDHashes = new List<MMDInfo>();
            Year = date.Year;
            Month = date.Month;
        }
    }
}
