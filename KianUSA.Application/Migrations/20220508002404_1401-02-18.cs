using Microsoft.EntityFrameworkCore.Migrations;

namespace KianUSA.Application.Migrations
{
    public partial class _14010218 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Inventory",
                table: "Product",
                type: "float",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Category_Order",
                table: "Category",
                column: "Order");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Category_Order",
                table: "Category");

            migrationBuilder.DropColumn(
                name: "Inventory",
                table: "Product");
        }
    }
}
