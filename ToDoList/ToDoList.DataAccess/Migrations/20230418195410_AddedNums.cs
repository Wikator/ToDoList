using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ToDoList.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedNums : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "English",
                table: "Groups");

            migrationBuilder.AddColumn<int>(
                name: "SubjectType",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GroupType",
                table: "Groups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 1,
                column: "GroupType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "GroupType", "Name" },
                values: new object[] { 3, "All groups" });

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 1,
                column: "SubjectType",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Subjects",
                keyColumn: "Id",
                keyValue: 2,
                column: "SubjectType",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubjectType",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "GroupType",
                table: "Groups");

            migrationBuilder.AddColumn<bool>(
                name: "English",
                table: "Groups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 1,
                column: "English",
                value: false);

            migrationBuilder.UpdateData(
                table: "Groups",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "English", "Name" },
                values: new object[] { true, "None" });

            migrationBuilder.InsertData(
                table: "Groups",
                columns: new[] { "Id", "English", "Name" },
                values: new object[,]
                {
                    { 3, false, "All groups" },
                    { 4, true, "All groups" }
                });
        }
    }
}
