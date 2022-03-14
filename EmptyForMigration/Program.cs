using System.Linq;

using IwaraDatabase;

namespace EmptyForMigration
{
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