using Microsoft.EntityFrameworkCore.Migrations;

namespace KianUSA.Application.Migrations
{
    public partial class _20230809 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReadyDate",
                table: "PoData",
                newName: "StatusDate");

            migrationBuilder.AlterColumn<double>(
                name: "Rate",
                table: "PoData",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FactoryContainerNumber",
                table: "PoData",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FactoryContainerNumber",
                table: "PoData");

            migrationBuilder.RenameColumn(
                name: "StatusDate",
                table: "PoData",
                newName: "ReadyDate");

            migrationBuilder.AlterColumn<string>(
                name: "Rate",
                table: "PoData",
                type: "text",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);
        }
    }
}
