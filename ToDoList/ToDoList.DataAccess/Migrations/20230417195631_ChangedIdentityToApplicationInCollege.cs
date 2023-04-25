using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoList.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangedIdentityToApplicationInCollege : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colleges_AspNetUsers_IdentityUserId",
                table: "Colleges");

            migrationBuilder.RenameColumn(
                name: "IdentityUserId",
                table: "Colleges",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Colleges_IdentityUserId",
                table: "Colleges",
                newName: "IX_Colleges_ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Colleges_AspNetUsers_ApplicationUserId",
                table: "Colleges",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colleges_AspNetUsers_ApplicationUserId",
                table: "Colleges");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Colleges",
                newName: "IdentityUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Colleges_ApplicationUserId",
                table: "Colleges",
                newName: "IX_Colleges_IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Colleges_AspNetUsers_IdentityUserId",
                table: "Colleges",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
