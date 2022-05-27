using Microsoft.EntityFrameworkCore.Migrations;

namespace KianUSA.Application.Migrations
{
    public partial class _14010229 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CategorySlug",
                table: "CategoryProduct",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategorySlug",
                table: "CategoryProduct");
        }
    }
}
