using Microsoft.EntityFrameworkCore.Migrations;

namespace KianUSA.Application.Migrations
{
    public partial class kianusa20240307 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "PoDataArchive");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "PoDataArchive");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "PoData");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PoDataArchive",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "PoDataArchive",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Date",
                table: "PoDataArchive",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DueDate",
                table: "PoDataArchive",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Date",
                table: "PoData",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DueDate",
                table: "PoData",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
