using Microsoft.EntityFrameworkCore.Migrations;

namespace KianUSA.Application.Migrations
{
    public partial class _14011210 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Prices",
                table: "Role",
                newName: "Buttons");

            migrationBuilder.AddColumn<string>(
                name: "Features",
                table: "Product",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PricePermissions",
                table: "Product",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Features",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "PricePermissions",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "Buttons",
                table: "Role",
                newName: "Prices");
        }
    }
}
