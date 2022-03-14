using IwaraDatabase.Entities;

using Microsoft.EntityFrameworkCore;

namespace IwaraDatabase
{
    public class Database : DbContext
    {
        private readonly string ConnectionString;
        public DbSet<MMDInfo> MMDInfos { set; get; }
        public DbSet<MonthInfo> MonthInfos { set; get; }

        /// <summary> 给 UWP 用 </summary>
        public Database () => ConnectionString = "Data Source=Hashes.db";

        /// <summary> 给 ConsoleApp 用 </summary>
        /// <param name="str"> 数据库路径 </param>
        public Database (string str) => ConnectionString = $"Data Source={str}";

        protected override void OnConfiguring (DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite(ConnectionString);
        }
    }
}