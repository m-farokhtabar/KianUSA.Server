using Microsoft.EntityFrameworkCore.Migrations;

namespace KianUSA.Application.Migrations
{
    public partial class _14010623 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Product_Order",
                table: "Product",
                column: "Order");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Product_Order",
                table: "Product");
        }
    }
}
