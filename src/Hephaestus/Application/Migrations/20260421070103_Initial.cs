using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hephaestus.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RawVacancies",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HeadHunterId = table.Column<int>(type: "integer", nullable: false),
                    VacancyName = table.Column<string>(type: "text", nullable: false),
                    VacancyDescription = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false),
                    IsProcessed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawVacancies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KeySkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    RawVacancyId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeySkills", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KeySkills_RawVacancies_RawVacancyId",
                        column: x => x.RawVacancyId,
                        principalTable: "RawVacancies",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_KeySkills_RawVacancyId",
                table: "KeySkills",
                column: "RawVacancyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KeySkills");

            migrationBuilder.DropTable(
                name: "RawVacancies");
        }
    }
}
