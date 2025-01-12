using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HireWireBackend.Storage.Migrations
{
    /// <inheritdoc />
    public partial class MigrationTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Tags__D5B6E3B69F1A1BA7", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "JobVacancyTags",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VacancyId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__JobVacan__3214EC07123F0EEC", x => x.Id);
                    table.ForeignKey(
                        name: "FK__JobVacanc__TagId__49C3F6B7",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__JobVacanc__Vacan__48CFD27E",
                        column: x => x.VacancyId,
                        principalTable: "JobVacancies",
                        principalColumn: "VacancyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobVacancyTags_TagId",
                table: "JobVacancyTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_JobVacancyTags_VacancyId",
                table: "JobVacancyTags",
                column: "VacancyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobVacancyTags");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
