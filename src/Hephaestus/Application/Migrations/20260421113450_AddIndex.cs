using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hephaestus.Migrations
{
    /// <inheritdoc />
    public partial class AddIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_RawVacancies_HeadHunterId",
                table: "RawVacancies",
                column: "HeadHunterId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RawVacancies_HeadHunterId",
                table: "RawVacancies");
        }
    }
}
