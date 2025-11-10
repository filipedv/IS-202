using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OBLIG1.Migrations
{
    /// <inheritdoc />
    public partial class AddRegisterforerRelationToObstacle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegisterforerId",
                table: "Obstacles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Registerforere",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Navn = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Epost = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registerforere", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Obstacles_RegisterforerId",
                table: "Obstacles",
                column: "RegisterforerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Obstacles_Registerforere_RegisterforerId",
                table: "Obstacles",
                column: "RegisterforerId",
                principalTable: "Registerforere",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Obstacles_Registerforere_RegisterforerId",
                table: "Obstacles");

            migrationBuilder.DropTable(
                name: "Registerforere");

            migrationBuilder.DropIndex(
                name: "IX_Obstacles_RegisterforerId",
                table: "Obstacles");

            migrationBuilder.DropColumn(
                name: "RegisterforerId",
                table: "Obstacles");
        }
    }
}
