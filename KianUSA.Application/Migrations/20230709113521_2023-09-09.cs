using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KianUSA.Application.Migrations
{
    public partial class _20230909 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ConfirmeStatus",
                table: "PoData",
                newName: "ShippmentStatus");

            migrationBuilder.AlterColumn<string>(
                name: "Rate",
                table: "PoData",
                type: "text",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BillDate",
                table: "PoData",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DischargeStatus",
                table: "PoData",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EmptyDate",
                table: "PoData",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GateIn",
                table: "PoData",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GateOut",
                table: "PoData",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PortOfDischarge",
                table: "PoData",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillDate",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "DischargeStatus",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "EmptyDate",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "GateIn",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "GateOut",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "PortOfDischarge",
                table: "PoData");

            migrationBuilder.RenameColumn(
                name: "ShippmentStatus",
                table: "PoData",
                newName: "ConfirmeStatus");

            migrationBuilder.AlterColumn<long>(
                name: "Rate",
                table: "PoData",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
