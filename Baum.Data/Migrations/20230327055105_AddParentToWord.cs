using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Baum.AvaloniaApp.Migrations
{
    /// <inheritdoc />
    public partial class AddParentToWord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Words",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Words_ParentId",
                table: "Words",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Words_Words_ParentId",
                table: "Words",
                column: "ParentId",
                principalTable: "Words",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Words_Words_ParentId",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_ParentId",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Words");
        }
    }
}
