using Microsoft.EntityFrameworkCore.Migrations;

namespace KianUSA.Application.Migrations
{
    public partial class _14010211 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Security",
                table: "Category");

            migrationBuilder.AddColumn<string>(
                name: "Parameter",
                table: "Category",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Parameter",
                table: "Category");

            migrationBuilder.AddColumn<string>(
                name: "Security",
                table: "Category",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }
    }
}
