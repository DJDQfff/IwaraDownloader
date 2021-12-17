using Microsoft.EntityFrameworkCore.Migrations;

namespace IwaraDownloader.Databases.Migrations
{
    public partial class init : Migration
    {
        protected override void Up (MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonthInfos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Year = table.Column<int>(nullable: false),
                    Month = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MMDInfos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Hash = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    UnixTimeStamp = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Heart = table.Column<int>(nullable: false),
                    EyeOpen = table.Column<int>(nullable: false),
                    WhetherDownloaded = table.Column<bool>(nullable: false),
                    MonthInfoId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MMDInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MMDInfos_MonthInfos_MonthInfoId",
                        column: x => x.MonthInfoId,
                        principalTable: "MonthInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MMDInfos_MonthInfoId",
                table: "MMDInfos",
                column: "MonthInfoId");
        }

        protected override void Down (MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MMDInfos");

            migrationBuilder.DropTable(
                name: "MonthInfos");
        }
    }
}