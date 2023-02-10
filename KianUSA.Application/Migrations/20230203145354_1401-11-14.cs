using Microsoft.EntityFrameworkCore.Migrations;

namespace KianUSA.Application.Migrations
{
    public partial class _14011114 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductDescription",
                table: "Product",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductDescription",
                table: "Product");
        }
    }
}
