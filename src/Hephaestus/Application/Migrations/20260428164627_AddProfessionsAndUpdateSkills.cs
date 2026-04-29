using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hephaestus.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionsAndUpdateSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Direction",
                table: "SkillsOnReview",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "SkillsOnReview",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProfessionId",
                table: "SkillsOnReview",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SkillType",
                table: "SkillsOnReview",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Direction",
                table: "CleanSkills",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "CleanSkills",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProfessionId",
                table: "CleanSkills",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SkillType",
                table: "CleanSkills",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Professions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Direction = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SkillsOnReview_ProfessionId",
                table: "SkillsOnReview",
                column: "ProfessionId");

            migrationBuilder.CreateIndex(
                name: "IX_CleanSkills_ProfessionId",
                table: "CleanSkills",
                column: "ProfessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CleanSkills_Professions_ProfessionId",
                table: "CleanSkills",
                column: "ProfessionId",
                principalTable: "Professions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_SkillsOnReview_Professions_ProfessionId",
                table: "SkillsOnReview",
                column: "ProfessionId",
                principalTable: "Professions",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CleanSkills_Professions_ProfessionId",
                table: "CleanSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_SkillsOnReview_Professions_ProfessionId",
                table: "SkillsOnReview");

            migrationBuilder.DropTable(
                name: "Professions");

            migrationBuilder.DropIndex(
                name: "IX_SkillsOnReview_ProfessionId",
                table: "SkillsOnReview");

            migrationBuilder.DropIndex(
                name: "IX_CleanSkills_ProfessionId",
                table: "CleanSkills");

            migrationBuilder.DropColumn(
                name: "Direction",
                table: "SkillsOnReview");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "SkillsOnReview");

            migrationBuilder.DropColumn(
                name: "ProfessionId",
                table: "SkillsOnReview");

            migrationBuilder.DropColumn(
                name: "SkillType",
                table: "SkillsOnReview");

            migrationBuilder.DropColumn(
                name: "Direction",
                table: "CleanSkills");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "CleanSkills");

            migrationBuilder.DropColumn(
                name: "ProfessionId",
                table: "CleanSkills");

            migrationBuilder.DropColumn(
                name: "SkillType",
                table: "CleanSkills");
        }
    }
}
