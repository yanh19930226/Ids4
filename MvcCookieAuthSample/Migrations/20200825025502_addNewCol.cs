using Microsoft.EntityFrameworkCore.Migrations;

namespace MvcCookieAuthSample.Migrations
{
    public partial class addNewCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NewCol",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewCol",
                table: "AspNetUsers");
        }
    }
}
