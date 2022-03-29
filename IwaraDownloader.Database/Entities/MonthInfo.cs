using System.Collections.Generic;

namespace IwaraDatabase.Entities
{
    /// <summary>
    /// 月份信息
    /// </summary>
    public class MonthInfo
    {
        /// <summary>
        /// 主键
        /// </summary>
        public int Id { set; get; }
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { set; get; }
        /// <summary>
        /// 月份
        /// </summary>
        public int Month { set; get; }
        /// <summary>
        /// 这个月的MMD
        /// </summary>
        public List<MMDInfo> MMDs { set; get; }
        /// <summary>
        /// 实例化List
        /// </summary>
        public MonthInfo ()
        {
            MMDs = new List<MMDInfo>();
        }
    }
}