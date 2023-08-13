using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace KianUSA.Application.Migrations
{
    public partial class _20230626 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PoData",
                columns: table => new
                {
                    PoNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    FactoryStatus = table.Column<int>(type: "integer", nullable: true),
                    ReadyDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FactoryBookingDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DocumentsSendOutDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ForwarderName = table.Column<int>(type: "integer", nullable: true),
                    BookingDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Rate = table.Column<long>(type: "bigint", nullable: true),
                    ETD = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ETA = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ConfirmeStatus = table.Column<int>(type: "integer", nullable: true),
                    ConfirmDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoData", x => x.PoNumber);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PoData");
        }
    }
}
