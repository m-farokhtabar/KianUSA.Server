using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace KianUSA.Application.Migrations
{
    public partial class _1401_04_23 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    CategorySlug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ParentCategoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentCategorySlug = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CategoryCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryCategory_Category_ParentCategoryId",
                        column: x => x.ParentCategoryId,
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCategory_CategoryId",
                table: "CategoryCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCategory_ParentCategoryId",
                table: "CategoryCategory",
                column: "ParentCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryCategory");
        }
    }
}
