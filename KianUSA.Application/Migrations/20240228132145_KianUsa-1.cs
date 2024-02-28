using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KianUSA.Application.Migrations
{
    public partial class KianUsa1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContainerNumber",
                table: "PoData",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerPO",
                table: "PoData",
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

            migrationBuilder.AddColumn<string>(
                name: "ETAAtPort",
                table: "PoData",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstimateNumber",
                table: "PoData",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Forwarder",
                table: "PoData",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IOR",
                table: "PoData",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemGroup",
                table: "PoData",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PoData",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rep",
                table: "PoData",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShipTo",
                table: "PoData",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShippingCarrier",
                table: "PoData",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "PoData",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PoDataArchive",
                columns: table => new
                {
                    PoNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Rep = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    User = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Date = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CustomerPO = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    EstimateNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DueDate = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ItemGroup = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    Forwarder = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    IOR = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ShipTo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ShippingCarrier = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ContainerNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    ETAAtPort = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FactoryStatus = table.Column<int>(type: "integer", nullable: true),
                    StatusDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FactoryContainerNumber = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    BookingDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DocumentsSendOutDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ForwarderName = table.Column<int>(type: "integer", nullable: true),
                    FactoryBookingDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Rate = table.Column<double>(type: "double precision", nullable: true),
                    ETD = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ETA = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    PortOfDischarge = table.Column<string>(type: "text", nullable: true),
                    DischargeStatus = table.Column<int>(type: "integer", nullable: true),
                    ShippmentStatus = table.Column<int>(type: "integer", nullable: true),
                    ConfirmDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    GateIn = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EmptyDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    GateOut = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BillDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoDataArchive", x => x.PoNumber);
                });

            migrationBuilder.CreateTable(
                name: "PoDataSecurity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PoNumber = table.Column<string>(type: "text", nullable: true),
                    User = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<string>(type: "text", nullable: true),
                    CustomerPO = table.Column<string>(type: "text", nullable: true),
                    EstimateNumber = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    DueDate = table.Column<string>(type: "text", nullable: true),
                    ItemGroup = table.Column<string>(type: "text", nullable: true),
                    Forwarder = table.Column<string>(type: "text", nullable: true),
                    IOR = table.Column<string>(type: "text", nullable: true),
                    ShipTo = table.Column<string>(type: "text", nullable: true),
                    ShippingCarrier = table.Column<string>(type: "text", nullable: true),
                    ContainerNumber = table.Column<string>(type: "text", nullable: true),
                    ETAAtPort = table.Column<string>(type: "text", nullable: true),
                    FactoryStatus = table.Column<string>(type: "text", nullable: true),
                    StatusDate = table.Column<string>(type: "text", nullable: true),
                    FactoryContainerNumber = table.Column<string>(type: "text", nullable: true),
                    BookingDate = table.Column<string>(type: "text", nullable: true),
                    DocumentsSendOutDate = table.Column<string>(type: "text", nullable: true),
                    ForwarderName = table.Column<string>(type: "text", nullable: true),
                    FactoryBookingDate = table.Column<string>(type: "text", nullable: true),
                    Rate = table.Column<string>(type: "text", nullable: true),
                    ETD = table.Column<string>(type: "text", nullable: true),
                    ETA = table.Column<string>(type: "text", nullable: true),
                    PortOfDischarge = table.Column<string>(type: "text", nullable: true),
                    DischargeStatus = table.Column<string>(type: "text", nullable: true),
                    ShippmentStatus = table.Column<string>(type: "text", nullable: true),
                    ConfirmDate = table.Column<string>(type: "text", nullable: true),
                    GateIn = table.Column<string>(type: "text", nullable: true),
                    EmptyDate = table.Column<string>(type: "text", nullable: true),
                    GateOut = table.Column<string>(type: "text", nullable: true),
                    BillDate = table.Column<string>(type: "text", nullable: true),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PoDataSecurity", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PoDataArchive");

            migrationBuilder.DropTable(
                name: "PoDataSecurity");

            migrationBuilder.DropColumn(
                name: "ContainerNumber",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "CustomerPO",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "ETAAtPort",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "EstimateNumber",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "Forwarder",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "IOR",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "ItemGroup",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "Rep",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "ShipTo",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "ShippingCarrier",
                table: "PoData");

            migrationBuilder.DropColumn(
                name: "User",
                table: "PoData");
        }
    }
}
