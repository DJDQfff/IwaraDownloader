using System;
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;
namespace IwaraDownloader.Models
{
    public class MMDInfo
    {
        public int Id { set; get; }
        public string Hash { set; get; }
        public string UnixTimeStamp { set; get; }
        public string Username { set; get; }
        public string Title { set; get; }
        public int Heart { set; get; }
        public int EyeOpen { set; get; }
        public bool WhetherDownloaded { set; get; }
    }
}
