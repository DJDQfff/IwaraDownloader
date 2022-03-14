using System.Collections.Generic;

namespace IwaraDatabase.Entities
{
    public class MonthInfo
    {
        public int Id { set; get; }
        public int Year { set; get; }
        public int Month { set; get; }
        public List<MMDInfo> MMDs { set; get; }

        public MonthInfo ()
        {
            MMDs = new List<MMDInfo>();
        }
    }
}