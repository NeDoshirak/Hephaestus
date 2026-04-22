using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hephaestus.Application.Migrations
{
    /// <inheritdoc />
    public partial class AddSkiils : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CleanSkills",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Counter = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CleanSkills", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillsOnReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OriginalName = table.Column<string>(type: "text", nullable: false),
                    NormalizedName = table.Column<string>(type: "text", nullable: false),
                    Counter = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    SuggestedDisplayName = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillsOnReview", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SkillRelations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentSkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChildSkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    RelationType = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillRelations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillRelations_CleanSkills_ChildSkillId",
                        column: x => x.ChildSkillId,
                        principalTable: "CleanSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SkillRelations_CleanSkills_ParentSkillId",
                        column: x => x.ParentSkillId,
                        principalTable: "CleanSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SkillSynonyms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CleanSkillId = table.Column<Guid>(type: "uuid", nullable: false),
                    SynonymName = table.Column<string>(type: "text", nullable: false),
                    IsFromNormalization = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SkillSynonyms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SkillSynonyms_CleanSkills_CleanSkillId",
                        column: x => x.CleanSkillId,
                        principalTable: "CleanSkills",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CleanSkills_NormalizedName",
                table: "CleanSkills",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SkillRelations_ChildSkillId",
                table: "SkillRelations",
                column: "ChildSkillId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillRelations_ParentSkillId",
                table: "SkillRelations",
                column: "ParentSkillId");

            migrationBuilder.CreateIndex(
                name: "IX_SkillSynonyms_CleanSkillId",
                table: "SkillSynonyms",
                column: "CleanSkillId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SkillRelations");

            migrationBuilder.DropTable(
                name: "SkillsOnReview");

            migrationBuilder.DropTable(
                name: "SkillSynonyms");

            migrationBuilder.DropTable(
                name: "CleanSkills");
        }
    }
}
