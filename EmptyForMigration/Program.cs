using System.Linq;

using IwaraDatabase;

namespace EmptyForMigration
{
    /// <summary> 数据库操作控制台 </summary>
    internal class Program
    {
        private static readonly Database database = new Database(Configuration.path);

        private static void Main ()
        {
            SetAllNotDownloaded(false);
            System.Console.Read();
        }

        private static void SetAllNotDownloaded (bool b)
        {
            var all = database.MMDInfos.ToList();
            foreach (var a in all)
            {
                a.WhetherDownloaded = b;
            }
            database.SaveChanges();
        }
    }
}