using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Baum.AvaloniaApp.Migrations
{
    /// <inheritdoc />
    public partial class RenameParentToAncestor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Words_Words_ParentId",
                table: "Words");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "Words",
                newName: "AncestorId");

            migrationBuilder.RenameIndex(
                name: "IX_Words_ParentId",
                table: "Words",
                newName: "IX_Words_AncestorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Words_Words_AncestorId",
                table: "Words",
                column: "AncestorId",
                principalTable: "Words",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Words_Words_AncestorId",
                table: "Words");

            migrationBuilder.RenameColumn(
                name: "AncestorId",
                table: "Words",
                newName: "ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Words_AncestorId",
                table: "Words",
                newName: "IX_Words_ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Words_Words_ParentId",
                table: "Words",
                column: "ParentId",
                principalTable: "Words",
                principalColumn: "Id");
        }
    }
}
