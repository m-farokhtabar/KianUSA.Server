using Microsoft.EntityFrameworkCore.Migrations;

namespace KianUSA.Application.Migrations
{
    public partial class _140102112 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fob",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "FobSac",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "WH",
                table: "Product");

            migrationBuilder.AddColumn<string>(
                name: "Price",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Product");

            migrationBuilder.AddColumn<decimal>(
                name: "Fob",
                table: "Product",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "FobSac",
                table: "Product",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "WH",
                table: "Product",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
